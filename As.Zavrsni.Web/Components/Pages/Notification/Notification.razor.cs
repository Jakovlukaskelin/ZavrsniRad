using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Notifikacije.Command;
using As.Zavrsni.Aplication.Notifikacije.Model;
using As.Zavrsni.Aplication.Notifikacije.Query;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace As.Zavrsni.Web.Components.Pages.Notification
{
    public partial class Notification
    {
        private List<NotificationModel> notifications;

        [Inject]
        private IZavrsniDbContext DbContext { get; set; }
        [Inject]
        private IMediator Mediator { get; set; }

        protected override async Task OnInitializedAsync()
        {

            notifications = await Mediator.Send(new GetNotificationQuery());
            notifications = notifications
                            .Where(n => n.Quantity < 10 || (n.ExpiryDate.HasValue && n.ExpiryDate.Value <= DateOnly.FromDateTime(DateTime.Today.AddDays(3))))
                            .ToList();
        }

        private async Task MarkAsAddressed(int notificationId, bool isChecked)
        {
            if (isChecked)
            {
                var notification = notifications.FirstOrDefault(n => n.NotificationId == notificationId);
                if (notification != null)
                {
                    notification.Status = true;
                    await Mediator.Send(new MarkNotificationAsAddressedCommand { NotificationId = notificationId });
                    notifications.Remove(notification);
                   
                }
            }
        }

        private string GetBorderColor(NotificationModel notification)
        {
            if (notification.Quantity < 10)
            {
                return "border-red-500";
            }
            else if (notification.ExpiryDate.HasValue && notification.ExpiryDate.Value <= DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
            {
                return "border-yellow-500";
            }

            return "border-gray-300";
        }
    }
}
