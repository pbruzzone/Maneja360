<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Recuperacion.aspx.cs" Inherits="Maneja360.Pages.Recuperacion" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Recuperación Base de Datos</h1>
    <asp:Label ID="errorLabel" runat="server" />
    <asp:Panel ID="recPanel" runat="server" Visible="False">
        <div class="mb-3 row text-danger">
            <div class="col-md-6">
                <%= GetErrors() %>
            </div>
        </div>
        <div class="mb-3 row">
            <div class="col-md-6">
                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="backupFile">Elija archivo de backup a restaurar</asp:Label>
                <asp:FileUpload ID="backupFile" runat="server" CssClass="form-control" accept=".bak" />
            </div>
        </div>
        <div class="mb-3 row">
            <div class="col-md-6">
                <asp:Button ID="restoreButton" CssClass="btn btn-primary" Text="Restaurar" runat="server" OnClick="restoreButton_OnClick"  />
                <asp:Button ID="backupButton" CssClass="btn btn-primary" Text="Respaldar" runat="server" OnClick="backupButton_OnClick"  />
                <asp:Button ID="recalcularButton" CssClass="btn btn-primary" Text="Recalcular DV" runat="server" OnClick="recalcularButton_OnClick"   />
                <p><asp:Label ID="errorMsg" ForeColor="red" runat="server"></asp:Label></p>
            </div>
        </div>
    </asp:Panel>
</asp:Content>

