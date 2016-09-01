<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Hotcakes.Modules.AddToCartModule.View" %>
<asp:Panel runat="server" ID="pnlAddProduct">
    <p><%=GetLocalizedString("AddToCartMessage") %></p>
    <p><%= GetLocalizedString("ProductPrefix") %>: <asp:Label ID="lblProduct" runat="server"/></p>
    <p><asp:Button runat="server" ID="btnAddToCart" CssClass="dnnPrimaryAction" OnClick="lnkAddToCart_OnClick"/></p>
</asp:Panel>
<asp:Panel runat="server" ID="pnlNoProducts">
    <p><%=GetLocalizedString("NoProductsMessage") %></p>
</asp:Panel>