using As.Zavrsni.Aplication.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Notifikacije.Command
{
    public class MarkNotificationAsAddressedCommand : IRequest<Unit>  
    {
        public int NotificationId { get; set; }
    }

    public class MarkNotificationAsAddressedCommandHandler : IRequestHandler<MarkNotificationAsAddressedCommand, Unit>
    {
        private readonly IZavrsniDbContext _context;

        public MarkNotificationAsAddressedCommandHandler(IZavrsniDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(MarkNotificationAsAddressedCommand request, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications.FindAsync(request.NotificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

