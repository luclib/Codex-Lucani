<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Conjugator.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    Enter the verb you wish to conjugate below.&nbsp;
    <p>
        <asp:TextBox ID="txtVerbToConjugate" runat="server" Width="200px"></asp:TextBox>
        <br />
        <asp:Label ID="lblInfo" ForeColor="Red" runat="server"></asp:Label>
    </p>

    <p>
        Please select a voice:
        <asp:RadioButtonList ID="lstVoice" RepeatDirection="Horizontal" runat="server">
            <asp:ListItem Value="0" Text="Active" Selected="true"></asp:ListItem>
            <asp:ListItem Value="1" Text="Passive"></asp:ListItem>
        </asp:RadioButtonList>
    </p>

    <p>
        Please select a tense:<br />
        <asp:RadioButtonList ID="lstTenses" RepeatDirection="Horizontal" runat="server">
            <asp:ListItem ID="chkPresent" Text="Present" Value="0" Selected="True" runat="server"></asp:ListItem>
            <asp:ListItem ID="chkImperfect" Text="Imperfect"  Value="1" Enabled="true" runat="server"></asp:ListItem>
            <asp:ListItem ID="chkFuture" Text="Future" Value="2" Enabled="true" runat="server"></asp:ListItem>
        </asp:RadioButtonList> 
    </p>

    <p>
        Please a select a mood:<br />
        <asp:RadioButtonList ID="lstMoods" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem ID="chkIndicative" Text=Indicative Value="0" Selected="True" runat="server"></asp:ListItem>
            <asp:ListItem ID="chkImperative" Text=Imperative Value="1" runat="server"></asp:ListItem>
            <asp:ListItem ID="chkSubjunctive" Text=Subjunctive Value="2" Enabled="false" runat="server"></asp:ListItem>
            <asp:ListItem ID="chkParticiple" Text="Participle" Enabled="false" runat="server"></asp:ListItem>
        </asp:RadioButtonList>
    </p>

    <p>
        <asp:Button ID="btnConjugate" Text="Conjugate" runat="server" onclick="btnConjugate_Click"/>
    </p>

    <asp:ValidationSummary ID="ValidationSummary" runat="server"/>

    <div>
        <asp:Label ID="lblDefinition" runat="server">
            <b>Definition: </b>
        </asp:Label><br /><br />
        <table id="tbConjugation" border="1">
            <tr id="Person">
                <th></th>
                <th>Singular</th>
                <th>Plural</th>
            </tr>
            <tr id="tr1stPerson">
                <th class="RowHeaders">
                   1<sup>st</sup> Person 
                </th>
                <td><asp:Literal ID="FirstPersonSingularActivePresent" runat="server"></asp:Literal></td>
                <td><asp:Literal ID="firstPersonPlural" runat="server"></asp:Literal></td>
            </tr>
            <tr id="tr2ndPerson">
                <th class="RowHeaders">
                    2<sup>nd</sup> Person
                </th>
                <td><asp:Literal ID="secondPersonSingular" runat="server"></asp:Literal></td>
                <td><asp:Literal ID="secondPersonPlural" runat="server"></asp:Literal></td>
            </tr>
            <tr id="tr3rdPerson">
                <th class="RowHeaders">
                    3<sup>rd</sup> Person
                </th>
                <td><asp:Literal ID="thirdPersonSingular" runat="server"></asp:Literal></td>
                <td><asp:Literal ID="thirdPersonPlural" runat="server"></asp:Literal></td>
            </tr>
        </table>
    </div>
</asp:Content>

