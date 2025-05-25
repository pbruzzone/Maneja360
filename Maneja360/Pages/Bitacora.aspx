<%@ Page Title="Bitácora de Eventos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Bitacora.aspx.cs" Inherits="Maneja360.Pages.Bitacora" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 class="mb-3" id="title"><%: Title %></h2>
        <section id="bitacora-filter">
            <div class="row">
                <h4></h4>
                <hr />
                <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                    <p class="text-danger">
                        <asp:Literal runat="server" ID="FailureText" />
                    </p>
                </asp:PlaceHolder>
                <div class="row row-cols-lg-auto mb-3 align-items-center">
                    <div class="col-md-2">
                        <asp:Label runat="server" AssociatedControlID="NombreUsuario" CssClass="form-label">Nombre usuario</asp:Label>
                        <asp:TextBox runat="server" ID="NombreUsuario" CssClass="form-control" />
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server" AssociatedControlID="ComboModulo" CssClass="form-label">Modulo</asp:Label>
                        <asp:DropDownList runat="server" ID="ComboModulo" CssClass="form-control"  DataTextField="Nombre" DataValueField="Codigo" AutoPostBack="True" OnSelectedIndexChanged="ComboModulo_OnSelectedIndexChanged" />
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server" AssociatedControlID="ComboEvento" CssClass="form-label">Evento</asp:Label>
                        <asp:DropDownList runat="server" ID="ComboEvento" CssClass="form-control"  DataTextField="Descripcion" DataValueField="Codigo" />
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server" AssociatedControlID="FechaDesde" CssClass="form-label">Desde</asp:Label>
                        <asp:TextBox runat="server" ID="FechaDesde" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server" AssociatedControlID="FechaHasta" CssClass="form-label">Hasta</asp:Label>
                        <asp:TextBox runat="server" ID="FechaHasta" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server" AssociatedControlID="ComboCriticidad" CssClass="form-label">Criticidad</asp:Label>
                        <asp:DropDownList runat="server" ID="ComboCriticidad" CssClass="form-control" />
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server" CssClass="form-label">Acciones</asp:Label>
                        <div class="mt-2">
                            <asp:Button runat="server" ID="Filtrar" CssClass="col-auto btn btn-primary btn-sm" Text="Filtrar" OnClick="Filtrar_OnClick"/>
                            <asp:Button runat="server" ID="Limpiar" CssClass="col-auto btn btn-secondary btn-sm" Text="Limpiar" OnClick="Limpiar_OnClick"/>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <hr />
        <section id="bitacora-list" class="row mb-3">
            <asp:GridView ID="GridBitacora" runat="server" CssClass="table table-striped" AutoGenerateColumns="False" OnRowCreated="BitacoraGrid_RowCreated">
                <Columns>
                    <asp:BoundField DataField="NombreUsuario" HeaderText="Usuario"  />
                    <asp:BoundField DataField="Modulo" HeaderText="Módulo" />
                    <asp:BoundField DataField="Evento" HeaderText="Evento" />
                    <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="Hora" HeaderText="Hora" DataFormatString="{0:hh\:mm}"  />
                    <asp:BoundField DataField="Criticidad" HeaderText="Criticidad" />
                </Columns>
            </asp:GridView>
        </section>
    </main>
</asp:Content>
