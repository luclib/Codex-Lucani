using System;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Collections.Generic;

public partial class _Default : System.Web.UI.Page
{
    protected void btnConjugate_Click(object sender, EventArgs e)
    {
        // Clear the labels of any remaining text.
        ClearText();

        // Create a string array to hold definitions, if the verb in 
        // question possesses more than one.
        List<string> definitions = new List<string>();

        if ((lstTenses.SelectedIndex != -1) && (lstMoods.SelectedIndex != -1) &&(lstVoice.SelectedIndex != -1))
        {
            if (CheckImpossibleConjugations())
            {

                // Define the ADO.NET Connection object.
                SqlConnection con = new SqlConnection();

                // Assign a connection string to the SqlConnection instance.
                con.ConnectionString = WebConfigurationManager.ConnectionStrings["LatinDictionary"].ConnectionString;

                if (txtVerbToConjugate.Text != "")
                {
                    // Get Latin targetWord from the text box.
                    string targetWord = txtVerbToConjugate.Text.ToLower();

                    // Create the SQL that will match the targetWord with the a noun from the Nouns table and retrieve
                    // the relevant info necessary to decline the word...
                    string selectSQL = "SELECT FirstPersonSingularActivePresent AS Name, Conjugation, VerbDefinitions.Definition, HasPrefix, EndsInEorY, KeepsEorY, DoubleConsonant, PastParticiple FROM Verbs INNER JOIN VerbDefinitions ON Verbs.VerbID = VerbDefinitions.VerbID WHERE FirstPersonSingularActivePresent = @Verb OR PresentInfinitive = @Verb OR VerbDefinitions.Definition = @Verb";

                    // Define the commander object using the connection object and the SQL statement.
                    SqlCommand cmd = new SqlCommand(selectSQL, con);

                    // Add the two parameters to the comander object.
                    cmd.Parameters.AddWithValue("@Verb", targetWord);

                    // Define SqlAdapter object.
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    // Define a new data set.
                    DataSet ds = new DataSet();

                    // Try to open the database and read the information.
                    try
                    {
                        using (con)
                        {
                            con.Open();

                            // Get row count.
                            int rowCount = adapter.Fill(ds, "VerbInfo");

                            // If the row count returns zero, then the application could not find the
                            // target word in the dictionary, else proceed to create a Verb object
                            // out of the verb info
                            if (rowCount != 0)
                            {
                                // If the user has entered a Latin word that has ONLY one definition
                                // in English, then the application will create a Verb object from the
                                // data set.
                                if (rowCount == 1)
                                {
                                    DataRow row = ds.Tables["VerbInfo"].Rows[0];

                                    // Create a new verb object using the info from the data row
                                    Verb verb = new Verb(
                                        row["Name"].ToString(),
                                        row["Definition"].ToString(),
                                        (VerbConjugation)row["Conjugation"],
                                        (bool)row["HasPrefix"],
                                        (bool)row["EndsInEorY"],
                                        (bool)row["KeepsEorY"],
                                        (bool)row["DoubleConsonant"],
                                        row["PastParticiple"].ToString()                               
                                        );

                                    // Obtain the verb tenses and moods from the radio button lists.
                                    verb.Voice = (Voice)int.Parse(lstVoice.SelectedValue);
                                    verb.Tense = (Tense)int.Parse(lstTenses.SelectedValue);
                                    verb.Mood = (Mood)int.Parse(lstMoods.SelectedValue);

                                    // Post definiton of the adjective:
                                    lblDefinition.Text += "to " + verb.Definition;

                                    /* Else, simply obtain the conjugations using the GetConjugations method
                                     * from the base Verb class.
                                     */
                                    this.PostConjugations(verb);
                                }
                                else
                                {
                                    // Else, if the user has enterd a Latin word with multiple translations
                                    // in English, the application will cycle through each of the data row, 
                                    // extract each definition and dump it into the List<string> created earlier.
                                    foreach (DataRow dRow in ds.Tables["VerbInfo"].Rows)
                                    {
                                        definitions.Add(dRow["Definition"].ToString());
                                    }
                                    // Then, add each definition to the definition label.
                                    foreach (string definition in definitions)
                                    {
                                        // Post the English word "to" in front of the first definition
                                        // to indicate that it is shown in the present infinitive.
                                        if (definitions.IndexOf(definition) == 0)
                                            lblDefinition.Text += "to " + definition + ", ";
                                        else if (definitions.IndexOf(definition) != definitions.Count - 1)
                                            lblDefinition.Text += definition + ", ";
                                        else
                                            lblDefinition.Text += definition;
                                    }

                                    // Once the definition label is completed, take the relevant info
                                    // from the first Data Row.
                                    DataRow row = ds.Tables["VerbInfo"].Rows[0];

                                    // Create a new verb object using the info from the data row
                                    Verb verb = new Verb(
                                        row["Name"].ToString(),
                                        row["Definition"].ToString(),
                                        (VerbConjugation)row["Conjugation"],
                                        (bool)row["HasPrefix"],
                                        (bool)row["EndsInEorY"],
                                        (bool)row["KeepsEorY"],
                                        (bool)row["DoubleConsonant"],
                                        row["PastParticiple"].ToString()
                                        );

                                    // Obtain the verb tenses and moods from the radio button lists.
                                    verb.Voice = (Voice)int.Parse(lstVoice.SelectedValue);
                                    verb.Tense = (Tense)int.Parse(lstTenses.SelectedValue);
                                    verb.Mood = (Mood)int.Parse(lstMoods.SelectedValue);

                                    this.PostConjugations(verb);
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
                else
                {
                    lblInfo.Text = "Please enter a verb into the text box above.";
                }
            }
            else
            {
                lblInfo.Text = "Error: the imperative mood can only be conjugated in the present tense.";
            }
        }
        else
        {
            lblInfo.Text = "Please select a tense, voice <u>and</u> mood from the options below.";
            lblInfo.Text += "<br/> Or else, we won't know how what to conjugate the verb!";
        }
    }

    /* The PostConjugations method will post all of the conjugations of a Latin verb, 
     * according to its Tense and Mood, into the cells of the HTML table according
     * to their person (first, second, and third) and number (singular or plural) along
     * with its English translations.
     * 
     * In doing so it will look for three thing:
     *    1) Is the English translation a composite verb (see comments below)
     *    2) Does the English translation of the verb contain an "e" at the end
     *       of its infinitive tense.
     *    3) If so, does it keep that "e" in the present continuous. For example:
     *       "to be" becomes "is being".
     */
    protected void PostConjugations(Verb verb)
    {
        string[,] conjugations = verb.GetConjugations();

        /* Check to see if the verb is a composite verb, meaning a verb that includes
         * the use of an adverb in its English translation as in "move up" or "take off"  */
        bool isComposite = verb.Definition.Contains(" ");

        if (isComposite)
        {
            /* So, using the example "give up", the first step would be to isolate the 
             * verb "give" from its adverb "up" by splitting the infinitive using the
             * Split() method and storing just the verb to the verb definition.      */
            char[] delim = { ' ' };
            string[] tokens = verb.Definition.Split(delim);
            verb.Definition = tokens[0];

            /* Now we store the remainder of the verb, usually just an adverb to a new 
             * string that will be later attached to the conjugations.      */
            string remainder ="";
            for (int i = 1; i < tokens.Length; i++)
                remainder += tokens[i] + " ";
            remainder.TrimEnd();

            /* Check to see if the english translation of the verb has a en "e" at the 
             * end. If that is the case, check to see if it keeps the e in its spelling
             * in the present continuous.*/
            if (verb.EndsInEorY)
            {
                if (verb.KeepsEorY)
                {
                    switch (verb.Voice)
                    {
                        case Voice.Active:
                            // Check the tense of the verb 
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". Define the present simple, present continuous,
                                 * and the present used with the verb "do".    */
                                case Tense.Present:

                                    // Check for the mood of the verb
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            string presentSimple = verb.Definition + " " + remainder;
                                            string presentContinuous = verb.Definition + "ing " + remainder;
                                            string presentDo = "do " + verb.Definition + " " + remainder;

                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] +
                                                                       "<br/><i>I " + presentSimple +
                                                                       " / am " + presentContinuous +
                                                                       " / " + presentDo + "</i>";
                                            firstPersonPlural.Text = conjugations[0, 1] +
                                                                    "<br/><i>we " + presentSimple +
                                                                    " /  are " + presentContinuous +
                                                                    " / " + presentDo + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] +
                                                                        "<br/><i>you " + presentSimple +
                                                                        " / are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] +
                                                                      "<br/><i>you " + presentSimple +
                                                                      " / are " + presentContinuous +
                                                                      " / " + presentDo + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] +
                                                                        "<br/><i>he / she / it " + presentSimple + "s" +
                                                                        " / is " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] +
                                                                        "<br/><i>they " + presentSimple +
                                                                        " /  are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            break;
                                        case Mood.Imperative:
                                            // Post conjugations
                                            string Imperative = verb.Definition + "!";
                                            secondPersonSingular.Text = conjugations[1, 0] +
                                                                        " <br/><i>" + Imperative + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] +
                                                                      " <br/><i>" + Imperative + "</i>";
                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    {
                                        /* Use the example of "to free". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string imperfect = verb.Definition + "ing " + remainder;

                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + imperfect + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + imperfect + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + imperfect + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + imperfect + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it was " + imperfect + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + imperfect + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                                break;
                                        }
                                    }
                                    break;
                                case Tense.Future:
                                    string future = "will " + verb.Definition;
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            {
                                                // Post conjugations
                                                FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I " + future + "</i>";
                                                firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we " + future + "</i>";

                                                secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you " + future + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you " + future + "</i>";

                                                thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it " + future + "</i>";
                                                thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they " + future + "</i>";
                                            }
                                            break;
                                        case Mood.Imperative:
                                            lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case Voice.Passive:
                            switch (verb.Tense)
                            {
                                case Tense.Present:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            {
                                                /* Use the example of "to free". */
                                                string pastParticiple = tokens[0];
                                            }
                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            {
                                                /* Use the example of "to free". */

                                                // Post conjugations
                                                FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + verb.PastParticiple +
                                                                           " / being " + verb.PastParticiple + "</i>"; ;
                                                firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were" + verb.PastParticiple + " / being " +
                                                                            verb.PastParticiple + "</i>";

                                                secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                                thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                                thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            }
                                            break;
                                    }
                                    break;
                                case Tense.Future:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            {
                                                /* Use the example of "to free". */

                                                // Post conjugations
                                                FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I will be " + verb.PastParticiple + "</i>"; ;
                                                firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we will be " + verb.PastParticiple + "</i>";

                                                secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you will be " + verb.PastParticiple + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you will be " + verb.PastParticiple + "</i>";

                                                thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it will be " + verb.PastParticiple + "</i>";
                                                thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they will be " + verb.PastParticiple + "</i>";
                                            }
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    /* For verbs that have an "e" as the last letter of their infinitive
                     * but drop the "e" when conjugating in the present continous or imperfect
                     * tenses. */

                    // First, get the verb stem by dropping the "e" at the end of its infinitive.
                    string verbStem = verb.Definition.Substring(0, verb.Definition.Length - 1);

                    switch (verb.Voice)
                    {
                        case Voice.Active:
                            // Check the tense of the verb 
                            switch (verb.Tense)
                            {
                                case Tense.Present:
                                    // Check for the mood of the verb
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            /* Use the example of "to give". Define the present simple, present continuous,
                                             * and the present used with the verb "do", making sure the verbStem of the verb
                                             * drops the "e" at the end of its infinitive. */
                                            string presentSimple = verb.Definition + " " + remainder;
                                            string presentContinuous = verbStem + "ing " + remainder;
                                            string presentDo = "do " + presentSimple;

                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] +
                                                                           "<br/><i>I " + presentSimple +
                                                                           " / am " + presentContinuous +
                                                                           " / " + presentDo + "</i>";
                                            firstPersonPlural.Text = conjugations[0, 1] +
                                                                       "<br/><i>we " + presentSimple +
                                                                       " /  are " + presentContinuous +
                                                                       " / " + presentDo + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] +
                                                                        "<br/><i>you " + presentSimple +
                                                                        " / are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] +
                                                                      "<br/><i>you " + presentSimple +
                                                                      " / are " + presentContinuous +
                                                                      " / " + presentDo + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] +
                                                                        "<br/><i>he / she / it " + presentSimple + "s" +
                                                                        " / is " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] +
                                                                        "<br/><i>they " + presentSimple +
                                                                        " /  are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            break;
                                        case Mood.Imperative:
                                            // Post conjugations
                                            string Imperative = verb.Definition + "!";
                                            secondPersonSingular.Text = conjugations[1, 0] +
                                                                        " <br/><i>" + Imperative + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] +
                                                                      " <br/><i>" + Imperative + "</i>";
                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    {
                                        /* Use the example of "to relate". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string imperfect = verbStem + "ing " + remainder;

                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + imperfect + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + imperfect + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + imperfect + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + imperfect + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it was " + imperfect + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + imperfect + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                                break;
                                        }
                                    }
                                    break;
                                case Tense.Future:
                                    {
                                        string future = "will " + verb.Definition;
                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I " + future + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we " + future + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you " + future + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you " + future + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it " + future + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they " + future + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                                break;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Voice.Passive:
                            switch (verb.Tense)
                            {
                                case Tense.Present:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            {
                                                /* Use the example of "to give". */

                                                // Post conjugations
                                                FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I am " + verb.PastParticiple +
                                                                           " / being " + verb.PastParticiple + "</i>"; ;
                                                firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we are " + verb.PastParticiple + " / being " +
                                                                            verb.PastParticiple + "</i>";

                                                secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                                thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it is " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                                thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            }
                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            {
                                                // Post conjugations
                                                FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + verb.PastParticiple +
                                                                           " / being " + verb.PastParticiple + "</i>"; ;
                                                firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were" + verb.PastParticiple + " / being " +
                                                                            verb.PastParticiple + "</i>";

                                                secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                                thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                                thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            }
                                            break;
                                    }
                                    break;
                                case Tense.Future:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            {
                                                /* Use the example of "to free".*/
                                                // Post conjugations
                                                FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I will be " + verb.PastParticiple + "</i>"; ;
                                                firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we will be " + verb.PastParticiple + "</i>";

                                                secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you will be " + verb.PastParticiple + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you will be " + verb.PastParticiple + "</i>";

                                                thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it will be " + verb.PastParticiple + "</i>";
                                                thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they will be " + verb.PastParticiple + "</i>";
                                            }
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                   
                }
            }
            else
            {
                /* If the word ends in a consonant, then we must check whether it takes
                 * double consonants in the present continuous or imperfect tenses, such
                 * the verb "put" or "win", whose forms would correspond to "winning" or
                 * "putting". As opposed to a verb like "disregard" that would only contain
                 * one consonant at the end in the imperfect or present continuous such
                 * "was disregarding" or "I am disregarding".  */
                if (verb.DoubleConsonant)
                {
                    // Use the verb "put" as the model.
                    char consonant = verb.Definition[verb.Definition.Length - 1];

                    switch (verb.Voice)
                    {
                        case Voice.Active:
                            // Check the tense of the verb 
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". Define the present simple, present continuous,
                                 * and the present used with the verb "do".    */
                                case Tense.Present:
                                    // Check for the mood of the verb
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            string presentSimple = verb.Definition + " " + remainder;
                                            string presentContinuous = verb.Definition + consonant + "ing " + remainder;
                                            string presentDo = "do " + verb.Definition;
                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] +
                                                                       "<br/><i>I " + presentSimple +
                                                                       " / am " + presentContinuous +
                                                                       " / " + presentDo + "</i>";
                                            firstPersonPlural.Text = conjugations[0, 1] +
                                                                    "<br/><i>we " + presentSimple +
                                                                    " /  are " + presentContinuous +
                                                                    " / " + presentDo + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] +
                                                                        "<br/><i>you " + presentSimple +
                                                                        " / are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] +
                                                                      "<br/><i>you " + presentSimple +
                                                                      " / are " + presentContinuous +
                                                                      " / " + presentDo + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] +
                                                                        "<br/><i>he / she / it " + presentSimple + "s" +
                                                                        " / is " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] +
                                                                        "<br/><i>they " + presentSimple +
                                                                        " /  are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            break;
                                        case Mood.Imperative:
                                            // Post conjugations
                                            string Imperative = verb.Definition + "!";
                                            secondPersonSingular.Text = conjugations[1, 0] +
                                                                        " <br/><i>" + Imperative + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] +
                                                                      " <br/><i>" + Imperative + "</i>";
                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    {
                                        /* Use the example of "to free". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string imperfect = verb.Definition + consonant + "ing";

                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + imperfect + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + imperfect + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + imperfect + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + imperfect + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it was " + imperfect + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + imperfect + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                                break;
                                        }
                                    }
                                    break;
                                case Tense.Future:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            break;
                                        case Mood.Imperative:
                                            lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case Voice.Passive:
                            break;
                    }
                }
                else
                {
                    // Use the verb "disregard" as the model

                    switch (verb.Voice)
                    {
                        case Voice.Active:
                            // Check the tense of the verb 
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". Define the present simple, present continuous,
                                 * and the present used with the verb "do".    */
                                case Tense.Present:
                                    {
                                        // Check for the mood of the verb
                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                string presentSimple = verb.Definition;
                                                string presentContinuous = verb.Definition + "ing";
                                                string presentDo = "do " + verb.Definition;

                                                // Post conjugations
                                                FirstPersonSingularActivePresent.Text = conjugations[0, 0] +
                                                                           "<br/><i>I " + presentSimple +
                                                                           " / am " + presentContinuous +
                                                                           " / " + presentDo + "</i>";
                                                firstPersonPlural.Text = conjugations[0, 1] +
                                                                        "<br/><i>we " + presentSimple +
                                                                        " /  are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";

                                                secondPersonSingular.Text = conjugations[1, 0] +
                                                                            "<br/><i>you " + presentSimple +
                                                                            " / are " + presentContinuous +
                                                                            " / " + presentDo + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] +
                                                                          "<br/><i>you " + presentSimple +
                                                                          " / are " + presentContinuous +
                                                                          " / " + presentDo + "</i>";

                                                thirdPersonSingular.Text = conjugations[2, 0] +
                                                                            "<br/><i>he / she / it " + presentSimple + "s" +
                                                                            " / is " + presentContinuous +
                                                                            " / " + presentDo + "</i>";
                                                thirdPersonPlural.Text = conjugations[2, 1] +
                                                                            "<br/><i>they " + presentSimple +
                                                                            " /  are " + presentContinuous +
                                                                            " / " + presentDo + "</i>";
                                                break;
                                            case Mood.Imperative:
                                                // Post conjugations
                                                string Imperative = verb.Definition + "!";
                                                secondPersonSingular.Text = conjugations[1, 0] +
                                                                            " <br/><i>" + Imperative + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] +
                                                                          " <br/><i>" + Imperative + "</i>";
                                                break;
                                        }
                                    }

                                    break;
                                case Tense.Imperfect:
                                    {
                                        /* Use the example of "to free". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string imperfect = verb.Definition + "ing";

                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + imperfect + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + imperfect + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + imperfect + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + imperfect + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it was " + imperfect + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + imperfect + "</i>";
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case Tense.Future:
                                    {
                                        /* Use the example of "to put". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string future = "will " + verb.Definition;
                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I " + future + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we " + future + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you " + future + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you " + future + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it " + future + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they " + future + "</i>";
                                                }
                                                break;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Voice.Passive:
                            break;
                    }
                }
            }
        }

        else
        {
            /* Check to see if the english translation of the verb has a en "e" at the 
             * end. If that is the case, check to see if it keeps the e in its spelling
             * in the present continuous.*/
            if (verb.EndsInEorY)
            {
                if (verb.KeepsEorY)
                {
                    switch (verb.Voice)
                    {
                        case Voice.Active:
                            // Check the tense of the verb 
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". Define the present simple, present continuous,
                                 * and the present used with the verb "do".    */
                                case Tense.Present:
                                    {
                                        // Check for the mood of the verb
                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                string presentSimple = verb.Definition;
                                                string presentContinuous = verb.Definition + "ing";
                                                string presentDo = "do " + verb.Definition;

                                                // Post conjugations
                                                FirstPersonSingularActivePresent.Text = conjugations[0, 0] +
                                                                           "<br/><i>I " + presentSimple +
                                                                           " / am " + presentContinuous +
                                                                           " / " + presentDo + "</i>";
                                                firstPersonPlural.Text = conjugations[0, 1] +
                                                                        "<br/><i>we " + presentSimple +
                                                                        " /  are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";

                                                secondPersonSingular.Text = conjugations[1, 0] +
                                                                            "<br/><i>you " + presentSimple +
                                                                            " / are " + presentContinuous +
                                                                            " / " + presentDo + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] +
                                                                          "<br/><i>you " + presentSimple +
                                                                          " / are " + presentContinuous +
                                                                          " / " + presentDo + "</i>";

                                                thirdPersonSingular.Text = conjugations[2, 0] +
                                                                            "<br/><i>he / she / it " + presentSimple + "s" +
                                                                            " / is " + presentContinuous +
                                                                            " / " + presentDo + "</i>";
                                                thirdPersonPlural.Text = conjugations[2, 1] +
                                                                            "<br/><i>they " + presentSimple +
                                                                            " /  are " + presentContinuous +
                                                                            " / " + presentDo + "</i>";
                                                break;
                                            case Mood.Imperative:
                                                // Post conjugations
                                                string Imperative = verb.Definition + "!";
                                                secondPersonSingular.Text = conjugations[1, 0] +
                                                                            " <br/><i>" + Imperative + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] +
                                                                          " <br/><i>" + Imperative + "</i>";
                                                break;
                                        }
                                    }
                                    break;
                                case Tense.Imperfect:
                                    {
                                        /* Use the example of "to free". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string imperfect = verb.Definition + "ing";

                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + imperfect + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + imperfect + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + imperfect + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + imperfect + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it was " + imperfect + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + imperfect + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                
                                                break;
                                        }
                                    }
                                    break;
                                case Tense.Future:
                                    {
                                        /* Use the example of "to free". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string future = "will " + verb.Definition;
                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I " + future + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we " + future + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you " + future + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you " + future + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it " + future + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they " + future + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                                break;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Voice.Passive:
                            // Check the tense of the verb
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". */
                                case Tense.Present:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I am " + verb.PastParticiple +
                                                                       " / being " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we are " + verb.PastParticiple + " / being " +
                                                                        verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it is " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    // Post conjugations
                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + verb.PastParticiple +
                                                               " / being " + verb.PastParticiple + "</i>"; ;
                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + verb.PastParticiple + " / being " +
                                                                verb.PastParticiple + "</i>";

                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                    break;
                                case Tense.Future:
                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I will be " + verb.PastParticiple + "</i>"; ;
                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we will be " + verb.PastParticiple + "</i>";

                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + "</i>";
                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + "</i>";

                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + "</i>";
                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + "</i>";
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    /* For verbs that have an "e" as the last letter of their infinitive
                     * but drop the "e" when conjugating in the present continous or imperfect
                     * tenses. Use the example of "to give".  */

                    // First, get the verb stem by dropping the "e" at the end of its infinitive.
                    string verbStem = verb.Definition.Substring(0, verb.Definition.Length - 1);

                    // Check the voice of the verb
                    switch (verb.Voice)
                    {
                        case Voice.Active:
                            // Check the tense of the verb 
                            switch (verb.Tense)
                            {
                                case Tense.Present:
                                    // Check for the mood of the verb
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            /* Use the example of "to give". Define the present simple, present continuous,
                                             * and the present used with the verb "do", making sure the verbStem of the verb
                                             * drops the "e" at the end of its infinitive. */
                                            string presentSimple = verb.Definition;
                                            string presentContinuous = verbStem + "ing";
                                            string presentDo = "do " + presentSimple;

                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] +
                                                                           "<br/><i>I " + presentSimple +
                                                                           " / am " + presentContinuous +
                                                                           " / " + presentDo + "</i>";
                                            firstPersonPlural.Text = conjugations[0, 1] +
                                                                       "<br/><i>we " + presentSimple +
                                                                       " /  are " + presentContinuous +
                                                                       " / " + presentDo + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] +
                                                                        "<br/><i>you " + presentSimple +
                                                                        " / are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] +
                                                                      "<br/><i>you " + presentSimple +
                                                                      " / are " + presentContinuous +
                                                                      " / " + presentDo + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] +
                                                                        "<br/><i>he / she / it " + presentSimple + "s" +
                                                                        " / is " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] +
                                                                        "<br/><i>they " + presentSimple +
                                                                        " /  are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    {
                                        /* Use the example of "to relate". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string imperfect = verbStem + "ing";

                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + imperfect + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + imperfect + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + imperfect + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + imperfect + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it was " + imperfect + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + imperfect + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                                break;
                                        }
                                    }
                                    break;
                                case Tense.Future:
                                    {
                                        /* Use the example of "to give". Define the future tense by attaching 
                                         * "will " to the beginning of the verb's definition.  */
                                        string future = "will " + verb.Definition;
                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I " + future + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we " + future + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you " + future + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you " + future + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it " + future + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they " + future + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                                break;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Voice.Passive:
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". */
                                case Tense.Present:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I am " + verb.PastParticiple +
                                                                       " / being " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we are " + verb.PastParticiple + " / being " +
                                                                        verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it is " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + verb.PastParticiple +
                                                                       " / being " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + verb.PastParticiple + " / being " +
                                                                        verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            break;
                                    }
                                    break;
                                case Tense.Future:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I will be " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we will be " + verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + "</i>";
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            /* Now we need to post conjugations for verbs that don't end in "e" in 
             * the English infinitive. In other words, all the verbs that end with
             * a consonant.   */
            else
            {
                /* If the word ends in a consonant, then we must check whether it takes
                 * double consonants in the present continuous or imperfect tenses, such
                 * the verb "put" or "win", whose forms would correspond to "winning" or
                 * "putting". As opposed to a verb like "disregard" that would only contain
                 * one consonant at the end in the imperfect or present continuous such
                 * "was disregarding" or "I am disregarding".  */
                if (verb.DoubleConsonant)
                {
                    // Use the verb "put" as the model.
                    char consonant = verb.Definition[verb.Definition.Length - 1];

                    switch (verb.Voice)
                    {
                        case Voice.Active:
                            // Check the tense of the verb 
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". Define the present simple, present continuous,
                                 * and the present used with the verb "do".    */
                                case Tense.Present:
                                    {
                                        // Check for the mood of the verb
                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                string presentSimple = verb.Definition;
                                                string presentContinuous = verb.Definition + consonant + "ing";
                                                string presentDo = "do " + verb.Definition;
                                                
                                                // Post conjugations
                                                FirstPersonSingularActivePresent.Text = conjugations[0, 0] +
                                                                           "<br/><i>I " + presentSimple +
                                                                           " / am " + presentContinuous +
                                                                           " / " + presentDo + "</i>";
                                                firstPersonPlural.Text = conjugations[0, 1] +
                                                                        "<br/><i>we " + presentSimple +
                                                                        " /  are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";

                                                secondPersonSingular.Text = conjugations[1, 0] +
                                                                            "<br/><i>you " + presentSimple +
                                                                            " / are " + presentContinuous +
                                                                            " / " + presentDo + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] +
                                                                          "<br/><i>you " + presentSimple +
                                                                          " / are " + presentContinuous +
                                                                          " / " + presentDo + "</i>";

                                                thirdPersonSingular.Text = conjugations[2, 0] +
                                                                            "<br/><i>he / she / it " + presentSimple + "s" +
                                                                            " / is " + presentContinuous +
                                                                            " / " + presentDo + "</i>";
                                                thirdPersonPlural.Text = conjugations[2, 1] +
                                                                            "<br/><i>they " + presentSimple +
                                                                            " /  are " + presentContinuous +
                                                                            " / " + presentDo + "</i>";
                                                break;
                                            case Mood.Imperative:
                                                // Post conjugations
                                                string Imperative = verb.Definition + "!";
                                                secondPersonSingular.Text = conjugations[1, 0] +
                                                                            " <br/><i>" + Imperative + "</i>";
                                                secondPersonPlural.Text = conjugations[1, 1] +
                                                                          " <br/><i>" + Imperative + "</i>";
                                                break;
                                        }
                                    }

                                    break;
                                case Tense.Imperfect:
                                    {
                                        /* Use the example of "to free". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string imperfect = verb.Definition + consonant + "ing";

                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + imperfect + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + imperfect + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + imperfect + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + imperfect + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it was " + imperfect + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + imperfect + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                                break;
                                        }
                                    }
                                    break;
                                case Tense.Future:
                                    {
                                        /* Use the example of "to put". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string future = "will " + verb.Definition;
                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I " + future + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we " + future + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you " + future + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you " + future + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it " + future + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they " + future + "</i>";
                                                }
                                                break;
                                            case Mood.Imperative:
                                                lblInfo.Text = "Error: The imperative only exists in the present tense.";
                                                break;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Voice.Passive:
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". */
                                case Tense.Present:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I am " + verb.PastParticiple +
                                                                       " / being " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we are " + verb.PastParticiple + " / being " +
                                                                        verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it is " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + verb.PastParticiple +
                                                                       " / being " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + verb.PastParticiple + " / being " +
                                                                        verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            break;
                                    }
                                    break;
                                case Tense.Future:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I will be " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we will be " + verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + "</i>";
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    // Use the verb "disregard" as the model

                    // Check the VOICE of the verb
                    switch (verb.Voice)
                    {
                        case Voice.Active:
                            // Check the tense of the verb 
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". Define the present simple, present continuous,
                                 * and the present used with the verb "do".    */
                                case Tense.Present:
                                    // Check for the mood of the verb
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            string presentSimple = verb.Definition;
                                            string presentContinuous = verb.Definition + "ing";
                                            string presentDo = "do " + verb.Definition;
                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] +
                                                                       "<br/><i>I " + presentSimple +
                                                                       " / am " + presentContinuous +
                                                                       " / " + presentDo + "</i>";
                                            firstPersonPlural.Text = conjugations[0, 1] +
                                                                    "<br/><i>we " + presentSimple +
                                                                    " /  are " + presentContinuous +
                                                                    " / " + presentDo + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] +
                                                                        "<br/><i>you " + presentSimple +
                                                                        " / are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] +
                                                                      "<br/><i>you " + presentSimple +
                                                                      " / are " + presentContinuous +
                                                                      " / " + presentDo + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] +
                                                                        "<br/><i>he / she / it " + presentSimple + "s" +
                                                                        " / is " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] +
                                                                        "<br/><i>they " + presentSimple +
                                                                        " /  are " + presentContinuous +
                                                                        " / " + presentDo + "</i>";
                                            break;
                                        case Mood.Imperative:
                                            secondPersonSingular.Text = conjugations[1, 0] + " <br/><i>" + verb.Definition + "!</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + " <br/><i>" + verb.Definition + "!</i>";
                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    {
                                        /* Use the example of "to free". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string imperfect = verb.Definition + "ing";

                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + imperfect + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + imperfect + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + imperfect + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + imperfect + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it was " + imperfect + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + imperfect + "</i>";
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case Tense.Future:
                                    {
                                        /* Use the example of "to disregard". Define the imperfect tense by attaching
                                         * "ing" to the end of the verb infinitive.  */
                                        string future = "will " + verb.Definition;
                                        switch (verb.Mood)
                                        {
                                            case Mood.Indicative:
                                                {
                                                    // Post conjugations
                                                    FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I " + future + "</i>";
                                                    firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we " + future + "</i>";

                                                    secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you " + future + "</i>";
                                                    secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you " + future + "</i>";

                                                    thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he / she / it " + future + "</i>";
                                                    thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they " + future + "</i>";
                                                }
                                                break;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Voice.Passive:
                            switch (verb.Tense)
                            {
                                /* Use the example of "to free". */
                                case Tense.Present:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I am " + verb.PastParticiple +
                                                                       " / being " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we are " + verb.PastParticiple + " / being " +
                                                                        verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it is " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they are " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            break;
                                    }
                                    break;
                                case Tense.Imperfect:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            // Post conjugations
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I was " + verb.PastParticiple +
                                                                       " / being " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we were " + verb.PastParticiple + " / being " +
                                                                        verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + " / being " + verb.PastParticiple + "</i>";
                                            break;
                                    }
                                    break;
                                case Tense.Future:
                                    switch (verb.Mood)
                                    {
                                        case Mood.Indicative:
                                            FirstPersonSingularActivePresent.Text = conjugations[0, 0] + "<br/><i>I will be " + verb.PastParticiple + "</i>"; ;
                                            firstPersonPlural.Text = conjugations[0, 1] + "<br/><i>we will be " + verb.PastParticiple + "</i>";

                                            secondPersonSingular.Text = conjugations[1, 0] + "<br/><i>you were " + verb.PastParticiple + "</i>";
                                            secondPersonPlural.Text = conjugations[1, 1] + "<br/><i>you were " + verb.PastParticiple + "</i>";

                                            thirdPersonSingular.Text = conjugations[2, 0] + "<br/><i>he/she/it was " + verb.PastParticiple + "</i>";
                                            thirdPersonPlural.Text = conjugations[2, 1] + "<br/><i>they were " + verb.PastParticiple + "</i>";
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
        }
        
    }

    protected bool CheckImpossibleConjugations()
    {
        bool isPossible = true;
        if ( (lstMoods.SelectedItem == chkImperative) && ((lstTenses.SelectedItem == chkImperfect || lstTenses.SelectedItem == chkFuture)) )
        {
            isPossible = false;
        }
        return isPossible;
    }

    protected bool CheckAvailableConjugations()
    {
        bool isAvailable = true;

        return isAvailable;
    }

    protected void ClearText()
    {
        lblInfo.Text = "";
        lblDefinition.Text = "<b>Definition: </b>";

        FirstPersonSingularActivePresent.Text = "";
        firstPersonPlural.Text = "";

        secondPersonSingular.Text = "";
        secondPersonPlural.Text = "";

        thirdPersonSingular.Text = "";
        thirdPersonPlural.Text = "";
    }
}