using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Notifikacije.Model;
using As.Zavrsni.Domain.Entites;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Notifikacije.Query
{
    public class GetNotificationQuery : IRequest<List<NotificationModel>>
    {

    }

    public class GetNotificationQueryHandler : IRequestHandler<GetNotificationQuery, List<NotificationModel>>
    {
        private readonly IZavrsniDbContext _context;
        private readonly IMapper _mapper;

        public GetNotificationQueryHandler(IZavrsniDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<NotificationModel>> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
        {
            var notificationEntities = await _context.Notifications
                .Where(n => n.Status == "false")
                 .Include(p => p.Product)
                 .ProjectTo<NotificationModel>(_mapper.ConfigurationProvider)
                 .ToListAsync(cancellationToken);



            return notificationEntities;
        }

    }
}
