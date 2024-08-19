using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Notifikacije.Model;
using As.Zavrsni.Aplication.Notifikacije.Query;
using As.Zavrsni.Aplication.Orders;
using As.Zavrsni.Aplication.Orders.NewFolder.Query;
using As.Zavrsni.Aplication.Products.Model;
using As.Zavrsni.Domain.Entites;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace As.Zavrsni.Web.Components.Pages.Orders
{
    public partial class Orders : IDisposable
    {
        private List<OrderModel> orders;
        private bool isDialogVisible = false;
        private int modalOrderId;
        private List<ProductsModel> selectedProducts = new List<ProductsModel>();
        private bool isDisposed = false;

        [Inject]
        private IZavrsniDbContext DbContext { get; set; }
        [Inject]
        private IMediator Mediator { get; set; }

        protected override async Task OnInitializedAsync()
        {
            isDisposed = false;
            orders = await Mediator.Send(new GetOrdersQuery());
        }

        private async Task ShowOrderDetails(int orderId)
        {
            if (isDisposed) return;

            var selectedOrder = await Mediator.Send(new GetOrdersQuery(orderId));

            if (isDisposed) return;

            if (selectedOrder != null && selectedOrder.Count > 0)
            {
                var order = selectedOrder.First();
                modalOrderId = order.OrderId;
                selectedProducts = order.Products;
                isDialogVisible = true;

                if (isDisposed) return;
                StateHasChanged();
            }
        }

        private void CloseDialog()
        {
            if (isDisposed) return;
            isDialogVisible = false;
            StateHasChanged();
        }

        public void Dispose()
        {
            isDisposed = true;
        }
    }
}
