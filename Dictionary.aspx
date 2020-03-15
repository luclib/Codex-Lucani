<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Dictionary.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <h2>Dictionary</h2>
    <p>
        Search quickly for a word in the text box below<br />
        <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox><br />
        <asp:Label ID="lblInfo" ForeColor="Red" runat="server"></asp:Label>
    </p>
    <p>
        <asp:Button ID="cmdSearch" Text="Search" runat="server" OnClick="cmdSearch_Click"  /> &nbsp;&nbsp
        <asp:Button ID="cmRefresh" Text="Refresh" runat="server" OnClick="cmRefresh_Click" />
    </p>
    <asp:GridView ID="GridView" DataSourceID="sourceDictionary" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" AllowSorting="true" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None"
        BorderWidth="1px" CellPadding="3" CellSpacing="2" Width="75%">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Word" />
            <asp:BoundField DataField="PartOfSpeech" HeaderText="Part of Speech" 
                            ItemStyle-HorizontalAlign="Center">
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Definition" HeaderText="Definition" />
        </Columns>
        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FFF1D4" />
        <SortedAscendingHeaderStyle BackColor="#B95C30" />
        <SortedDescendingCellStyle BackColor="#F1E5CE" />
        <SortedDescendingHeaderStyle BackColor="#93451F" />
    </asp:GridView>
    <asp:SqlDataSource ID="sourceDictionary" runat="server"
        ConnectionString="<%$ ConnectionStrings:LatinDictionary %>"
        SelectCommand="Select Name, PartOfSpeech, Definition FROM Dictionary ORDER BY Name">
    </asp:SqlDataSource>
</asp:Content>

