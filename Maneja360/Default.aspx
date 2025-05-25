<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Maneja360._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <section class="row mb-3" aria-labelledby="productTitle">
            <h4 id="productTitle">Maneja360</h4>
            <p class="lead">Maneja360 es una aplicación web de gestión simple, que permita a los pequeños negocios llevar su operación diaria sin complicaciones.</p>
        </section>
        <section class="row">
            <h4>Perfiles del usuario</h4>
            <asp:TreeView ID="ProfileTreeView" runat="server" ShowLines="True" ShowExpandCollapse="False">
                <RootNodeStyle Font-Bold="True" />
            </asp:TreeView>
        </section>
    </main>

</asp:Content>

