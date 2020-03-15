<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AdjectiveDecliner.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    Enter the adjective you wish to decline below.<br />
    <p>
        <asp:TextBox ID="txtWordtoDecline" runat="server" Width="200px" TextMode="SingleLine"></asp:TextBox><br />
    </p>

    <p>
        <asp:ValidationSummary runat="server" ID="ValidationSummary" />
        <asp:Label ID="lblInfo" runat="server" ForeColor="Red"></asp:Label>
    </p>

    <p>
        <asp:Button ID="cmdDecline" Text="Decline" runat="server" OnClick="cmdDecline_Click" style="height: 26px"/>
    </p>       
    
    <div>
        <asp:Label ID="lblDefinition" runat="server">
            <b>Definition: </b>
        </asp:Label><br /><br />

         <table id="tbDeclension" border="1">
             <tr id="trPerson">
                 <th></th>
                 <th colspan="3">Singular</th>
                 <th colspan="3">Plural</th>
             </tr>
             <tr id="Gender">
                 <th></th>
                 <th><small>Masc.</small></th>
                 <th><small>Fem.</small></th>
                 <th><small>Neuter</small></th>

                 <th><small>Masc.</small></th>
                 <th><small>Fem.</small></th>
                 <th><small>Neuter</small></th>
             </tr>
             <tr id="Nominative">
                 <th>Nominative</th>
                 <td><asp:Literal ID="txtNomMascSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtNomFemSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtNomNeutSingular" runat="server"></asp:Literal></td>
                                  
                 <td><asp:Literal ID="txtNomMascPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtNomFemPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtNomNeutPlural" runat="server"></asp:Literal></td>
             </tr>
             <tr id="Genitive">
                 <th>Genitive</th>
                 <td><asp:Literal ID="txtGenMascSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtGenFemSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtGenNeutSingular" runat="server"></asp:Literal></td>
                                  
                 <td><asp:Literal ID="txtGenMascPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtGenFemPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtGenNeutPlural" runat="server"></asp:Literal></td>
             </tr>
            <tr id="Dative">
                 <th>Dative</th>
                 <td><asp:Literal ID="txtDatMascSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtDatFemSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtDatNeutSingular" runat="server"></asp:Literal></td>

                 <td><asp:Literal ID="txtDatMascPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtDatFemPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtDatNeutPlural" runat="server"></asp:Literal></td>
             </tr>
             <tr id="Accusative">
                 <th>Accusative</th>
                 <td><asp:Literal ID="txtAccMascSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtAccFemSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtAccNeutSingular" runat="server"></asp:Literal></td>

                 <td><asp:Literal ID="txtAccMascPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtAccFemPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtAccNeutPlural" runat="server"></asp:Literal></td>
             </tr>
             <tr id="Ablative">
                 <th>Ablative</th>
                 <td><asp:Literal ID="txtAblMascSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtAblFemSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtAblNeutSingular" runat="server"></asp:Literal></td>

                 <td><asp:Literal ID="txtAblMascPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtAblFemPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtAblNeutPlural" runat="server"></asp:Literal></td>
             </tr>
             <tr id="Vocative">
                 <th>Vocative</th>
                 <td><asp:Literal ID="txtVocMascSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtVocFemSingular" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtVocNeutSingular" runat="server"></asp:Literal></td>

                 <td><asp:Literal ID="txtVocMascPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtVocFemPlural" runat="server"></asp:Literal></td>
                 <td><asp:Literal ID="txtVocNeutPlural" runat="server"></asp:Literal></td>
             </tr>
         </table>
    </div>
</asp:Content>

