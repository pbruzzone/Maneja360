<%@ Page Title="Iniciar Sesión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Maneja360.Account.Login" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %></h2>
        <div class="col-md-8">
            <section id="loginForm">
                <div class="row">
                    <%--<h4>Usar una cuenta local para iniciar sesión.</h4>--%>
                    <hr />
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="NombreUsuario" CssClass="form-label">Nombre usuario</asp:Label>
                        <div class="col-md-6">
                            <asp:TextBox runat="server" ID="NombreUsuario" CssClass="form-control"  />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="NombreUsuario"
                                CssClass="text-danger" ErrorMessage="El campo nombre de usuario es requerido." />
                        </div>
                    </div>
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="Password" CssClass="form-label">Contraseña</asp:Label>
                        <div class="col-md-6">
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="El campo contraseña es requerido." />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-12">
                            <div class="form-checl">
                                <asp:CheckBox runat="server" ID="RememberMe" CssClass="form-check-input" />
                                <asp:Label runat="server" AssociatedControlID="RememberMe" CssClass="form-check-label">¿Recordarme?</asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-12">
                            <asp:Button runat="server" OnClick="LogIn" Text="Iniciar Sesión" CssClass="btn btn-primary" />
                        </div>
                    </div>
                   <%-- <div class="row mb-3">
                        <div class="col-md-12">
                            <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled" CssClass=".fs-5">Registrarse como un nuevo usuario</asp:HyperLink>
                        </div>
                    </div>--%>
                </div>
                <p>
                    <%-- Enable this once you have account confirmation enabled for password reset functionality
                    <asp:HyperLink runat="server" ID="ForgotPasswordHyperLink" ViewStateMode="Disabled">Forgot your password?</asp:HyperLink>
                    --%>
                </p>
            </section>
        </div>
    </main>
</asp:Content>
