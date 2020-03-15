using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;

public partial class _Default : System.Web.UI.Page
{
    // Define the ADO.NET Connection object.
   SqlConnection con = new SqlConnection();

    // Define a string to hold SQL statement.
    string selectSQL;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            selectSQL = "SELECT Name, PartOfSpeech, Definition FROM Dictionary ORDER BY Name";

            con.ConnectionString = WebConfigurationManager.ConnectionStrings["LatinDictionary"].ConnectionString;

            SqlCommand cmd = new SqlCommand(selectSQL, con);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            try
            {
                using (con)
                {
                    con.Open();

                    // Get row count.
                    int rowCount = adapter.Fill(ds, "Dictionary");
                    if (rowCount != 0)
                    {
                        // Create a new Data Table that will hold the Dictionary search results2 without
                        // the duplicate rows.
                        DataTable dt = new DataTable();
                        // Add corresponding Data columns.
                        dt.Columns.Add("Name", typeof(string));
                        dt.Columns.Add("PartOfSpeech", typeof(string));
                        dt.Columns.Add("Definition", typeof(string));

                        // Loop through each data row and remove any duplicats while lumping all 
                        // definitions in the first data row.
                        foreach (DataRow row in ds.Tables["Dictionary"].Rows)
                        {
                            if(dt.Rows.Count == 0)
                            {
                                DataRow newRow = dt.NewRow();
                                newRow["Name"] = row["Name"];
                                newRow["PartOfSpeech"] = row["PartOfSpeech"];
                                newRow["Definition"] = row["Definition"];


                                dt.Rows.Add(newRow);
                            }
                            else
                            {
                                // First search
                                var dtValue1 = row["Name"];
                                var dtValue2 = row["Definition"];

                                DataRow[] results = dt.Select("Name = '" + dtValue1 + "' AND Definition = '" + dtValue2 + "'");

                                if(results.Length > 0)
                                {
                                    // Extract the name and definition value of the row
                                    var value1 = row["Name"];
                                    var value2 = row["Definition"];

                                    // Look for a row with the same value.
                                    DataRow[] results2 = ds.Tables["Dictionary"].Select("Name = '" + value1 + "' AND Definition = '" + value2 + "'");

                                    // Check to see if the array results2 is larger than 1.
                                    if (results2.Length >= 2)
                                    {
                                        // If so, loop through each row, extract the definition and dump it into a new 
                                        DataRow entryRow = results2[0];

                                        // Define counter index
                                        int index;

                                        // Cycle through each row in the DataRow array.
                                        for (index = 1; index <= results2.Length - 1; index++)
                                        {
                                            if (index != results2.Length - 1)
                                                entryRow["Definition"] += ", " + results2[index]["Definition"].ToString();
                                            else
                                                entryRow["Definition"] += results2[index]["Definition"].ToString();
                                        }

                                        // Add the entry row to the new data table.
                                        dt.Rows.Add(entryRow);

                                    }
                                    // If the DataRow array is equal to one,
                                    // just add the current row to the new data table.
                                    else
                                    {
                                        DataRow newRow = dt.NewRow();
                                        newRow["Name"] = row["Name"];
                                        newRow["PartOfSpeech"] = row["PartOfSpeech"];
                                        newRow["Definition"] = row["Definition"];

                                        // Add the row to the new data table.
                                        dt.Rows.Add(newRow);
                                    }
                                }
                            }
                        }

                        GridView.DataSourceID = null;
                        GridView.DataSource = dt;
                        GridView.AllowPaging = true;
                        GridView.DataBind();
                    }
                }
            }
            catch(Exception ex)
            {
                lblInfo.Text = "Error loading dictionary.<br/>";
                lblInfo.Text += ex.Message;
            }
        }
    }

    protected void cmdSearch_Click(object sender, EventArgs e)
    {
        // Clear the info label.
        lblInfo.Text = "";

        // Assign a connection string to the SqlConnection instance.
        con.ConnectionString = WebConfigurationManager.ConnectionStrings["LatinDictionary"].ConnectionString;

        // Define the SQL select command.
         selectSQL = "SELECT Name, PartOfSpeech, Definition FROM Dictionary WHERE Name LIKE '%' + @Word + '%' OR Definition LIKE '%' + @Word + '%' ORDER BY Name";

        // Define an SqlCommand object using the SqlConnection and select Command statement.
        SqlCommand cmd = new SqlCommand(selectSQL, con);

        // Add the parameters to the SqlCommand object.
        cmd.Parameters.AddWithValue("@Word", txtSearch.Text);

        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

        DataSet ds = new DataSet();

        try
        {
            using (con)
            {
                con.Open();

                // Get row count.
                int rowCount = adapter.Fill(ds, "SearchResult");
                if (rowCount != 0)
                {
                    GridView.DataSourceID = null;
                    GridView.DataSource = ds.Tables["SearchResult"];
                    GridView.AllowPaging = true;
                    GridView.DataBind();
                }
                else
                {
                    // Display error message.
                    lblInfo.Text = "Could not find word in the dictionary. The following reasons might apply: <br/> <br/>";
                    lblInfo.Text += "•	Word is not yet in the dictionary (mea culpa…)<br/> <br/>";
                    lblInfo.Text += "•	You have misspelled the word <br/> <br/>";
                    lblInfo.Text += "•	You have written the word using the wrong grammatical case (try using word stems only).<br/> <br/>";
                }
            }          
        }
        catch(Exception ex)
        {
            lblInfo.Text = "Error reading dictionary.<br/>";
            lblInfo.Text += ex.Message;
        }
    }

    protected void cmRefresh_Click(object sender, EventArgs e)
    {
        GridView.DataSource = null;
        GridView.DataSourceID = sourceDictionary.ID;     
    }
}