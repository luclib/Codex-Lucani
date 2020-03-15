<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SovTranslator.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <p class="title">The SOV translator</p>

    <p>The <b>SOV</b> translator is the simplest translation application that takes a sentence containing just
        one <u><b>s</b></u>ubject, one <u><b>o</b></u>bject, and one <u><b>v</b></u>erb in English and translates into Latin(or vice-versa) in keeping with the general rules of syntax of both languages.
    </p>

    <p>In the case of English, the general syntax follows the SVO pattern, where the 
        subject always comes first, followed by the verb and then the object.<br />
        For example: The girl helps the farmer.
    </p>
     <p>In Latin, the subject comes first followed by the object and then the verb, as 
        in the following sentence (translating the previous example):<br />
        Puella iuvat agricolam.
     </p>
    
    <p>To get started, pop up the dicitonary window at the bottom of the navigation tree
        to your left, look up some words, and get started translating!
    </p>
    Enter sentence below
   <p>
       <asp:TextBox ID="txtToBeTranslated" runat="server" Width="200px" Height="25px"></asp:TextBox>
       <asp:RadioButtonList runat="server" ID="lstTranslationOrder" RepeatDirection="Horizontal">
           <asp:ListItem Value="0" Text="Latin to English"></asp:ListItem>
           <asp:ListItem Value="1" Text="English to Latin"></asp:ListItem>
       </asp:RadioButtonList>
       <br />
        <asp:CheckBox ID="chkDefArticle" Font-Size="14px" runat="server" Text="Use definitive articles" />
   </p>
    

    <p>
       
        <asp:Label ID="lblInfo" ForeColor="Red" runat="server"></asp:Label>
    </p>

    <p>
        
        <br />
        <asp:Button ID="cmdTranslate" Text="Translate" runat="server" OnClick="cmdTranslate_Click" />
    </p>
   
    <p>
        <asp:TextBox ID="txtTranslation" runat="server" width="200px"></asp:TextBox>
    </p><br />

</asp:Content>

