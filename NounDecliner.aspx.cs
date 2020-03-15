using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

public partial class _Default : System.Web.UI.Page
{

    protected void cmdDecline_Click(object sender, EventArgs e)
    {
        // Clear the info label of any remaning text.
        lblInfo.Text = "";

        // Define the ADO.NET Connection object.
        SqlConnection con = new SqlConnection();

        // Assign a connection string to the SqlConnection instance.
        con.ConnectionString = WebConfigurationManager.ConnectionStrings["LatinDictionary"].ConnectionString;

        // Get Latin targetWord from the text box.
        string targetWord = txtWordtoDecline.Text.ToLower();

        // Create the SQL that will match the targetWord with the a noun from the Nouns table and retrieve
        // the relevant info necessary to decline the word...
        string selectSQL = "SELECT GenitiveSingular AS Name, NominativeSingular, Gender, Declension, Definition, IsProperNoun, Plural, PluralOnly FROM Nouns INNER JOIN NounDefinitions ON Nouns.NounID = NounDefinitions.NounID WHERE GenitiveSingular = @Noun OR NominativeSingular = @Noun OR NounDefinitions.Definition = @Noun";

        SqlCommand cmd = new SqlCommand(selectSQL, con);

        // Add the search parameters.
        cmd.Parameters.AddWithValue("@Noun", targetWord);

        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

        DataSet ds = new DataSet();

        // Try to open the database and read the information.
        try
        {
            using (con)
            {
                con.Open();

                // Get row count.
                int rowCount = adapter.Fill(ds, "NounInfo");
                if (rowCount != 0)
                {
                    DataRow row = ds.Tables["NounInfo"].Rows[0];

                    Noun matchWord = new Noun(
                        row["Name"].ToString(),
                        row["NominativeSingular"].ToString(),
                        row["Definition"].ToString(),
                        row["Gender"].ToString(),
                        (Decl)row["Declension"],
                        row["Plural"].ToString(),
                        (bool)row["IsProperNoun"],
                        (bool)row["PluralOnly"]
                        );
                    
                    this.PostDeclensions(matchWord);

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

    // The GetDeclensions method will fill the table with the correct declensions
    // of the noun. First it will determine to which declension group a 
    // given noun belongs to. Next, it will determine the declensions according
    // to its gender (if the gender modifies the declension, such as in the verbs
    // of the second declension).
    private void PostDeclensions(Noun word)
    {
        // Get the declensions

        string[,] declensions = word.GetDeclensions();
        // If the translated word has an irregular plural, use it to list the plural 
        // translations
        // Check to see if the noun constitutes a proper noun.
        if (word.IsProperNoun)
        {
            NominativeSingular.Text = declensions[0, 0] + "<br/><i>" + word.Definition + "</i>";
            NominativePlural.Text = declensions[0, 1] + "<br/><i> " + word.Definition + "</i>";

            GenitiveSingular.Text = declensions[1, 0] + "<br/><i>of " + word.Definition + "</i>";
            GenitivePlural.Text = declensions[1, 1] + "<br/><i>of " + word.Definition + "</i>";

            DativeSingular.Text = declensions[2, 0] + "<br/><i>to/for " + word.Definition + "</i>";
            DativePlural.Text = declensions[2, 1] + "<br/><i>to/for " + word.Definition + "</i>";

            AccusativeSingular.Text = declensions[3, 0] + "<br/><i>" + word.Definition + "</i>";
            AccusativePlural.Text = declensions[3, 1] + "<br/><i>" + word.Definition + "</i>";

            AblativeSingular.Text = declensions[4, 0] + "<br/><i>by/with/in " + word.Definition + "</i>";
            AblativePlural.Text = declensions[4, 1] + "<br/><i>by/with/in " + word.Definition + "</i>";

            VocativeSingular.Text = declensions[5, 0] + "<br/><i>" + word.Definition + "</i>";
            VocativePlural.Text = declensions[5, 1] + "<br/><i>" + word.Definition + "</i>";
        }
        else
        {
            // If the word exists only in the plural form, do not fill any of the singular
            // table cells
            if (word.PluralOnly)
            {
                NominativeSingular.Text = "";
                NominativePlural.Text = declensions[0, 1] + "<br/><i> (the) " + word.Plural + "</i>";

                GenitiveSingular.Text = "";
                GenitivePlural.Text = declensions[1, 1] + "<br/><i>of (the) " + word.Plural + "</i>";

                DativeSingular.Text = "";
                DativePlural.Text = declensions[2, 1] + "<br/><i>to/for the " + word.Definition + "</i>";

                AccusativeSingular.Text = "";
                AccusativePlural.Text = declensions[3, 1] + "<br/><i>(the) " + word.Plural + "</i>";

                AblativeSingular.Text = "";
                AblativePlural.Text = declensions[4, 1] + "<br/><i>by/with/in (the) " + word.Plural + "</i>";

                VocativeSingular.Text = "";
                VocativePlural.Text = declensions[5, 1] + "<br/><i>" + word.Plural + "</i>";
            }
            else
            {
                NominativeSingular.Text = declensions[0, 0] + "<br/><i> a/the " + word.Definition + "</i>";
                NominativePlural.Text = declensions[0, 1] + "<br/><i> (the) " + word.Plural + "</i>";

                GenitiveSingular.Text = declensions[1, 0] + "<br/><i>of a/the " + word.Definition + "</i>";
                GenitivePlural.Text = declensions[1, 1] + "<br/><i>of (the) " + word.Plural + "</i>";

                DativeSingular.Text = declensions[2, 0] + "<br/><i>to/for a/the " + word.Definition + "</i>";
                DativePlural.Text = declensions[2, 1] + "<br/><i>to/for the " + word.Plural + "</i>";

                AccusativeSingular.Text = declensions[3, 0] + "<br/><i> a/the " + word.Definition + "</i>";
                AccusativePlural.Text = declensions[3, 1] + "<br/><i>(the) " + word.Plural + "</i>";

                AblativeSingular.Text = declensions[4, 0] + "<br/><i>by/with/in a/the " + word.Definition + "</i>";
                AblativePlural.Text = declensions[4, 1] + "<br/><i>by/with/in (the) " + word.Plural + "</i>";

                VocativeSingular.Text = declensions[5, 0] + "<br/><i>" + word.Definition + "</i>";
                VocativePlural.Text = declensions[5, 1] + "<br/><i>" + word.Plural + "</i>";
            }
        }
    }
}