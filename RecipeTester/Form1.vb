Imports RecipeLibrary

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using db As RecipeContext = New RecipeContext
            Dim c As Category = New Category With {.Description = "Soup"}
            Dim u As Unit = New Unit With {.Description = "cup"}
            db.Categories.Add(c)
            db.Units.Add(u)

            db.SaveChanges()
        End Using
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Using db As RecipeContext = New RecipeContext
            Dim r As Recipe = New Recipe With {.Title = "My First Recipe", .Instructions = "What to do..."}
            db.Recipes.Add(r)
            db.SaveChanges()
        End Using
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim p As IngredientParser = New IngredientParser

        Dim l As List(Of IngredientLine) = p.ParseBlock(TextBox1.Text)

        For Each i In l
            MsgBox(i.Ingredient)

        Next
    End Sub
End Class
