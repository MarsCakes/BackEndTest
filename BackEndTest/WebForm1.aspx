<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="BackEndTest.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="Soal1">
            <h1>
                Soal 1
            </h1>
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label">NIK-Nama: </asp:Label>
            <asp:DropDownList ID="DropDownList1" runat="server" 
                OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
            </asp:DropDownList><br />
            <asp:Label ID="Label2" runat="server" Text="Label">Tgl Absen</asp:Label>
            <asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
            <asp:Calendar 
                ID="Calendar1" 
                runat="server" 
                OnSelectionChanged="Calendar1_SelectionChanged">
            </asp:Calendar>
            <asp:Button ID="Button1" runat="server" Text="Save" OnClick="Button1_Click" />
        </div>
      </div>
        <div class="Soal2">
            <h1>
                Soal 2
            </h1>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="NIK" HeaderText="NIK" />
                    <asp:BoundField DataField="Nama" HeaderText="Name" />
                    <asp:BoundField DataField="TanggalAbsen" HeaderText="Date" 
                                    DataFormatString="{0:yyyy-MM-dd}" />
                </Columns>
            </asp:GridView>
        </div>

        <div class="Soal3">
            <h1>
                Soal 3
            </h1>
            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="True">
            </asp:GridView>
        </div>

        <div class="Soal4">
            <h1>
                Soal 4
            </h1>
            <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="True">
            </asp:GridView>
        </div>

    </form>
</body>
</html>
