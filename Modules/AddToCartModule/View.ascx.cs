/*
' Copyright (c) 2016 Hotcakes Commerce, LLC
'  All rights reserved.
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy 
' of this software and associated documentation files (the "Software"), to deal 
' in the Software without restriction, including without limitation the rights 
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
' copies of the Software, and to permit persons to whom the Software is 
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all 
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
' SOFTWARE.
*/

using System;
using System.Linq;
using DotNetNuke.Services.Exceptions;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Urls;

namespace Hotcakes.Modules.AddToCartModule
{
    public partial class View : AddToCartModuleBase
    {
        #region Private Members

        private Product product = null;

        #endregion

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack) BindData();
            }
            catch (Exception exc) 
            {
                // Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void lnkAddToCart_OnClick(object sender, EventArgs e)
        {
            AddProductToCart();
        }

        #endregion

        #region Helper Methods

        private void BindData()
        {
            LocalizeModule();

            pnlAddProduct.Visible = false;
            pnlNoProducts.Visible = false;

            // get a product that doesn't have any choices/variants assigned
            GetProductFromTheStore();
            
            if (product != null)
            {
                // show the product to the visitor
                lblProduct.Text = string.Format("({0}) {1}", product.Sku, product.ProductName);

                pnlAddProduct.Visible = true;
            }
            else
            {
                pnlNoProducts.Visible = true;
            }
        }

        private void LocalizeModule()
        {
            btnAddToCart.Text = GetLocalizedString("btnAddToCart");
        }

        private void GetProductFromTheStore()
        {
            // create a reference to the Hotcakes store
            var HccApp = HccAppHelper.InitHccApp();

            // get a collection of the products in the store already
            var products = HccApp.CatalogServices.Products.FindAllPaged(1, int.MaxValue);

            // make sure we don't throw an error
            if (products == null || !products.Any()) return;

            // see if any are available that don't have any choices assigned
            if (products.Any(p => !p.HasOptions() && !p.HasVariants()))
            {
                // find the first product returned that doesn't have options or variants
                product = products.FirstOrDefault(p => !p.HasOptions() && !p.HasVariants());
            }
        }

        private void AddProductToCart()
        {
            /*
            * Example based on the code at the following documentation page:
            * 
            * https://hotcakescommerce.zendesk.com/hc/en-us/articles/204725889-Add-a-Product-to-Cart-Programmatically
            * 
            */

            // create a reference to the Hotcakes store
            var HccApp = HccAppHelper.InitHccApp();

            // find the first product returned that doesn't have options or variants
            if (product == null)
            {
                GetProductFromTheStore();
            }

            // set the quantity
            var quantity = 1;

            // create a reference to the current shopping cart
            Order currentCart = HccApp.OrderServices.EnsureShoppingCart();

            // create a line item for the cart using the product
            LineItem li = product.ConvertToLineItem(HccApp, quantity, new OptionSelections());

            // add the line item to the current cart
            HccApp.AddToOrderWithCalculateAndSave(currentCart, li);

            // send the customer to the shopping cart page
            Response.Redirect(HccUrlBuilder.RouteHccUrl(HccRoute.Cart));
        }

        #endregion
    }
}
