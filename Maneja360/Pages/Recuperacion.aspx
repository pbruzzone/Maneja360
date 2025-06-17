<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Recuperacion.aspx.cs" Inherits="Maneja360.Pages.Recuperacion" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Recuperación Base de Datos</h1>
    <asp:Label ID="errorLabel" runat="server" />
    <asp:Panel ID="recPanel" runat="server" Visible="False">
        <div class="row mb-3 mt-3 text-danger">
            <div class="col-md-6">
                <%= GetErrors() %>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-3">
                <asp:Button ID="restoreButton" CssClass="btn btn-primary" Text="Restore" runat="server" OnClick="restoreButton_OnClick"  />
            </div>
        </div>
        <div class="row">
            <div class="col-md-3">
                <asp:Button ID="recalcularButton" CssClass="btn btn-primary" Text="Recalcular DV" runat="server" OnClick="recalcularButton_OnClick"   />
            </div>
        </div>
    </asp:Panel>
</asp:Content>

