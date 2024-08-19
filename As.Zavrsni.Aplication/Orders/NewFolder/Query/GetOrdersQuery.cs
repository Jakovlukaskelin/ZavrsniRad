using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Domain.Entites;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Orders.NewFolder.Query
{
    public class GetOrdersQuery : IRequest<List<OrderModel>>
    {
        public int? OrderId { get; }

        public GetOrdersQuery(int? orderId = null)
        {
            OrderId = orderId;
        }
    }

    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderModel>>
    {
        private readonly IZavrsniDbContext _context;
        private readonly IMapper _mapper;

        public GetOrdersQueryHandler(IZavrsniDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<OrderModel>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Order> query = _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Product);

            if (request.OrderId.HasValue)
            {
                query = query.Where(o => o.OrderId == request.OrderId.Value);
            }

            return await query
                .ProjectTo<OrderModel>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
    
}
