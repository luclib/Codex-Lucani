using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public partial class _Default : System.Web.UI.Page
{
    // FIELDS ===============================================================================>
    // Define the ADO.NET technologies.
    private SqlConnection con = new SqlConnection();
    private SqlCommand cmd;
    private SqlDataAdapter adapter;

    // CLICK-HANDLER EVENTS ==================================================================>
    protected void cmdTranslate_Click(object sender, EventArgs e)
    {
        con.ConnectionString = @"Data Source=(localdb)\ProjectsV13;AttachDbFilename=C:\Users\Lucas\source\repos\CodexLucani\LatinDictionary.mdf;Initial Catalog=LatinDictionary(C:);Integrated Security=True";
        
        // Clear any text from the Info label.
        lblInfo.Text = "";

        // Check for check box selection
        if(lstTranslationOrder.SelectedIndex != -1)
        {
            // Translate the sentence

            // Check which option was selected
            if(lstTranslationOrder.SelectedValue == "0")
            {
                // User has selected "Latin to English"

            }
            else if(lstTranslationOrder.SelectedValue == "1")
            {
                // User has selected "English to Latin"
                char[] delim = { ' ', ',', '.' };
                string target = txtToBeTranslated.Text.ToLower().Trim();

                string[] tokens = target.Split(delim);

                List<string> englishWords = new List<string>();

                // Cycle through each element in the tokens string array
                for (int index = 0; index <= tokens.Length - 1; index++)
                {
                    if (tokens[index] != "the" && tokens[index] != "a" && tokens[index] != "an")
                        englishWords.Add(tokens[index]);
                }
                
                // Make sure the sentence has only three words.
                if(englishWords.Count != 3)
                {

                }
                else
                {

                }
            }
        }
        else
        {
            // Display message
            lblInfo.Text = "Error: Please select one of the two options above";
        }
    }

    // METHODS ==========================================================================>
    public string GetVerbName(string names)
    {
        string verbName;

        char[] delim = { ' ', ',' };
        string[] tokens = names.Split(delim);
        verbName = tokens[1];

        return verbName;
    }
}