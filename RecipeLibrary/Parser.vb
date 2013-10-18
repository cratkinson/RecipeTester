Imports System.Data.Entity.Design.PluralizationServices
Public Class IngredientParser
    Private Enum ParseStage
        amount
        unit
        ingredient
        instruction
    End Enum
    Private _amount As Decimal = 0
    Private _unit As String = String.Empty
    Private _ingredient As List(Of String) = New List(Of String)
    Private _instruction As List(Of String) = New List(Of String)
    Private _consumed As Boolean = False
    Private _stage As ParseStage = ParseStage.amount
    Private _units As List(Of String) = New List(Of String)
    'Private _units As List(Of unit) = New List(Of String) From {"cup", "ounce", "tablespoon", "teaspoon", "pound", "lb", "gram", "stick", "clove", "slice", "pinch"}
    Public Sub New()
        Using db As RecipeContext = New RecipeContext
            _units = (From u In db.Units Select u.Description).ToList
        End Using
    End Sub

    Public Function Parse(aLine As String) As IngredientLine
        _amount = 0
        _unit = String.Empty
        _ingredient = New List(Of String)
        _instruction = New List(Of String)
        _stage = ParseStage.amount

        Dim tokens() As String = aLine.Split

        For Each token In tokens
            If token <> String.Empty Then
                Do
                    Select Case _stage
                        Case ParseStage.amount
                            _consumed = amount(token)
                        Case ParseStage.unit
                            _consumed = unit(token)
                        Case ParseStage.ingredient
                            _consumed = ingredient(token)
                        Case ParseStage.instruction
                            _consumed = instruction(token)
                    End Select
                Loop Until _consumed
            End If
        Next


        Dim line As IngredientLine = New IngredientLine
        With line
            .Amount = _amount
            .Unit = _unit
            .Ingredient = String.Join(" ", _ingredient)
            .Instruction = String.Join(" ", _instruction)
        End With
        Return line
    End Function
    Public Function ParseBlock(aBlock As String) As List(Of IngredientLine)
        Dim theList As New List(Of IngredientLine)

        Dim aList As List(Of String) = aBlock.Split(CChar(Environment.NewLine)).ToList
        For Each i In aList
            If i <> String.Empty Then
                theList.Add(Me.Parse(i))
            End If
        Next
        Return theList
    End Function
    Private Function amount(token As String) As Boolean
        Dim a As Decimal = 0
        _stage = ParseStage.unit
        If token.Contains("/") Then
            Dim r() As String = token.Split("/")
            If IsNumeric(r(0)) And IsNumeric(r(1)) Then
                a = CType(r(0), Integer) / CType(r(1), Integer)
            Else
                Return False
            End If
        ElseIf IsNumeric(token) Then
            a = CType(token, Decimal)
        Else
            Return False
        End If
        _amount += a
        Return True
    End Function
    Private Function unit(token As String) As Boolean
        If IsNumeric(token) Or token.Contains("/") Then
            _stage = ParseStage.amount
            Return False
        End If

        Dim p As PluralizationService = PluralizationService.CreateService(New System.Globalization.CultureInfo("en-US"))

        _stage = ParseStage.ingredient
        If _units.Contains(p.Singularize(token.ToLower)) Then
            _unit = token
            Return True
        Else
            Return False
        End If
    End Function
    Private Function ingredient(token As String) As Boolean
        If token.EndsWith(",") Then
            _stage = ParseStage.instruction
            _ingredient.Add(token.Substring(0, token.Length - 1))
        Else
            _ingredient.Add(token)
        End If
        Return True
    End Function
    Private Function instruction(token As String)
        _instruction.Add(token)
        Return True
    End Function
End Class
