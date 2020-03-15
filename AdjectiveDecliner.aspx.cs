using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

public partial class _Default : System.Web.UI.Page
{
    string defaultText = "<b>Definition: </b>";
    protected void cmdDecline_Click(object sender, EventArgs e)
    {
        // Clear the info and reset the definition labels of any remaning text.
        lblInfo.Text = "";
        lblDefinition.Text = "<b>Definition: </b>";


        // Define the ADO.NET Connection object.
        SqlConnection con = new SqlConnection();

        // Assign a connection string to the SqlConnection instance.
        con.ConnectionString = WebConfigurationManager.ConnectionStrings["LatinDictionary"].ConnectionString;

        // Get Latin targetWord from the text box.
        string targetWord = txtWordtoDecline.Text.ToLower();

        // Create a string array to hold definitions, if the adjective in 
        // question possesses more than one.
        List<string> definitions = new List<string>();

        // Create the SQL that will match the targetWord with an adjective from the
        // Adjectives table and retrieve the relevant info necessary to instantiate 
        // a adjective object.
        string selectSQL = "SELECT NominativeMasculine, NominativeFeminine AS Name, AdjectiveType, Definition FROM Adjectives INNER JOIN AdjectiveDefinitions ON Adjectives.AdjectiveID = AdjectiveDefinitions.AdjectiveID WHERE NominativeMasculine = @Adjective OR NominativeFeminine = @Adjective OR NominativeNeuter = @Adjective OR AdjectiveDefinitions.Definition = @Adjective";

        // Create an SqlCommand object using the select statement connection
        SqlCommand cmd = new SqlCommand(selectSQL, con);

        // Add the search parameters.
        cmd.Parameters.AddWithValue("@Adjective", targetWord);

        // Define a data adapter using the SqlCommand object
        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

        DataSet ds = new DataSet();

        // Try to opent the database and read the information.
        try
        {
            using (con)
            {
                con.Open();

                // Get row count.
                int rowCount = adapter.Fill(ds, "AdjectiveInfo");
                if (rowCount != 0)
                {
                    // Create a data row(s) that will contain the adjective's informatino
                    
                    // If the adapter returned only one row, then create a single data
                    // row and use the columns to instantiate a new Adjective objects. 
                    if(rowCount == 1)
                    {
                        DataRow row = ds.Tables["AdjectiveInfo"].Rows[0];

                        Adjective matchWord = new Adjective(
                            row["Name"].ToString(),
                            row["Definition"].ToString(),
                            row["NominativeMasculine"].ToString(),
                            (AdjType)row["AdjectiveType"]
                            );

                        // Post definiton of the adjective:
                        lblDefinition.Text += matchWord.Definition;
                        this.PostDeclensions(matchWord);
                        

                    }
                    // If there is more than one row, cycle through each row, extract the
                    // Definition of each and dump it into the definition string list.
                    else
                    {
                        foreach(DataRow dRow in ds.Tables["AdjectiveInfo"].Rows)
                        {
                            definitions.Add(dRow["Definition"].ToString());
                        }
                        // Then, add each definition to the definition label.
                        foreach(string definition in definitions)
                        {
                            if (definitions.IndexOf(definition) != definitions.Count - 1)
                                lblDefinition.Text += definition + ", ";
                            else
                                lblDefinition.Text += definition;
                        }
                        DataRow row = ds.Tables["AdjectiveInfo"].Rows[0];

                        Adjective matchWord = new Adjective(
                            row["Name"].ToString(),
                            row["Definition"].ToString(),
                            row["NominativeMasculine"].ToString(),
                            (AdjType)row["AdjectiveType"]                            
                            );

                        this.PostDeclensions(matchWord);
                    }
                }
                else
                {
                    // Display error message.
                    lblInfo.Text = "Could not find word in the dictionary. The following reasons might apply: <br/> <br/>";
                    lblInfo.Text += "•	Word is not yet in the dictionary (mea culpa…)<br/> <br/>";
                    lblInfo.Text += "•	You have entered an incorrect word type (e.g. a noun instead of an adjective) <br/> <br/>";
                    lblInfo.Text += "•	You have misspelled the word <br/> <br/>";
                    lblInfo.Text += "•	You have written the word using the wrong grammatical case (see dictionary for help).<br/> <br/>";
                }
            }
        }
        catch (Exception ex)
        {
            lblInfo.Text = "Error reading dictionary.<br/>";
            lblInfo.Text += ex.Message;
        }
    }

    public void PostDeclensions(Adjective word)
    {
        // Get the declensions.
        string[,] declensions = word.GetDeclensions();

        // Nominative
        txtNomMascSingular.Text = declensions[0, 0];
        txtNomMascPlural.Text = declensions[0, 3];

        txtNomFemSingular.Text = declensions[0, 1];
        txtNomFemPlural.Text = declensions[0, 4];

        txtNomNeutSingular.Text = declensions[0, 2];
        txtNomNeutPlural.Text = declensions[0, 5];

        // Genitive
        txtGenMascSingular.Text = declensions[1, 0];
        txtGenMascPlural.Text = declensions[1, 3];

        txtGenFemSingular.Text = declensions[1, 1];
        txtGenFemPlural.Text = declensions[1, 4];

        txtGenNeutSingular.Text = declensions[1,2];
        txtGenNeutPlural.Text = declensions[1, 5];

        // Dative
        txtDatMascSingular.Text = declensions[2, 0];
        txtDatMascPlural.Text = declensions[2, 3];

        txtDatFemSingular.Text = declensions[2, 1];
        txtDatFemPlural.Text = declensions[2, 4];

        txtDatNeutSingular.Text = declensions[2, 2];
        txtDatNeutPlural.Text = declensions[2, 5];

        // Accusative
        txtAccMascSingular.Text = declensions[3, 0];
        txtAccMascPlural.Text = declensions[3, 3];

        txtAccFemSingular.Text = declensions[3, 1];
        txtAccFemPlural.Text = declensions[3, 4];

        txtAccNeutSingular.Text = declensions[3, 2];
        txtAccNeutPlural.Text = declensions[3, 5];

        // Ablative
        txtAblMascSingular.Text = declensions[4, 0];
        txtAblMascPlural.Text = declensions[4, 3];

        txtAblFemSingular.Text = declensions[4, 1];
        txtAblFemPlural.Text = declensions[4, 4];

        txtAblNeutSingular.Text = declensions[4, 2];
        txtAblNeutPlural.Text = declensions[4, 5];

        // Vocative
        txtVocMascSingular.Text = declensions[5, 0];
        txtVocMascPlural.Text = declensions[5, 3];

        txtVocFemSingular.Text = declensions[5, 1];
        txtVocFemPlural.Text = declensions[5, 4];

        txtVocNeutSingular.Text = declensions[5, 2];
        txtVocNeutPlural.Text = declensions[5, 5];


    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}