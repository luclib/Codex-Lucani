<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <p>Welcome to the Codex Lucani!</p>
    
    <p>
        The Codex Lucani is a rudimentary <b>Latin-English</b> dictionary and translation application.
    </p>

    <p>
        To the left you will find a navigation panel displaying the capabilities of the application. The first
        is the <b>SOV Translator</b> that translates simple Latin sentences containing a subject, object, and verb 
        in the present indicative to its English equivalent (and vice versa?).
    </p>
    <p>
        The second item pops up the <b>Inflector</b>: its function is to list all of the spelling cases for a given Latin word
        accompanied by an English translation.
    </p>
    <p>
        Finally, the third item is <b>Dictionary</b> that lists a full list of Latin words and their corresponding definitions.
    </p>
</asp:Content>

