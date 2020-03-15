<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="NounDecliner.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    Enter the noun you wish to decline below.<br />
    <p>
        <asp:TextBox ID="txtWordtoDecline" runat="server" Width="200px" TextMode="SingleLine"></asp:TextBox><br />
        <asp:Label ID="lblInfo" runat="server" ForeColor="Red"></asp:Label>
    </p>

    <p>
        <asp:Button ID="cmdDecline" Text="Decline" runat="server" OnClick="cmdDecline_Click" style="height: 26px"/>
    </p>
    
    <asp:ValidationSummary runat="server" ID="ValidationSummary" />
    
    <div>
         <table id="tbDeclension" border="1">
             <tr id="trPerson">
                 <th></th>
                 <th>Singular</th>
                 <th>Plural</th>
             </tr>
             <tr id="trNominative">
                 <td id="tdNominative">
                     <b>Nominative</b>
                 </td>
                 <td>
                     <asp:Literal ID="NominativeSingular" runat="server"></asp:Literal>
                 </td>
                 <td>
                     <asp:Literal ID="NominativePlural" runat="server"></asp:Literal>
                 </td>
             </tr>
             <tr id="trGenitive">
                 <td id="tdGenitive">
                     <b>Genitive</b>
                 </td>
                 <td><asp:Literal ID="GenitiveSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="GenitivePlural" runat="server"></asp:Literal></td>
             </tr>
             <tr id="trDative">
                 <td id="tdDative">
                     <b>Dative</b>
                 </td>
                 <td><asp:Literal ID="DativeSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="DativePlural" runat="server"></asp:Literal></td>
             </tr>
             <tr id="trAccusative">
                 <td id="tdAccusative">
                     <b>Accusative</b>
                 </td>
                 <td><asp:Literal ID="AccusativeSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="AccusativePlural" runat="server"></asp:Literal></td>
             </tr>
             <tr id="trAblative">
                 <td id="tdAblative">
                     <b>Ablative</b>
                 </td>
                 <td><asp:Literal ID="AblativeSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="AblativePlural" runat="server"></asp:Literal></td>
             </tr>
             <tr id="trVocative">
                 <td id="tdVocative">
                     <b>Vocative</b>
                 </td>
                 <td><asp:Literal ID="VocativeSingular" runat="server"></asp:Literal></td>
                 <td ><asp:Literal ID="VocativePlural" runat="server"></asp:Literal></td>
             </tr>
         </table>
    </div>

</asp:Content>

