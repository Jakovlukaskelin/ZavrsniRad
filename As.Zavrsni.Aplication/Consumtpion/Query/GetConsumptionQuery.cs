using As.Zavrsni.Aplication.Consumtpion.Model;
using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Products.Model;
using As.Zavrsni.Aplication.Products.Query;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Consumtpion.Query
{
    public class GetConsumptionQuery : IRequest<List<ConsumptionModel>>
    {

    }

    public class GetConsumptionQueryHandler : IRequestHandler<GetConsumptionQuery, List<ConsumptionModel>>
    {
        private readonly IZavrsniDbContext _context;
        private readonly IMapper _mapper;

        public GetConsumptionQueryHandler(IZavrsniDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ConsumptionModel>> Handle(GetConsumptionQuery request, CancellationToken cancellationToken)
        {
           var result = this._context.Consumptions
               .AsNoTracking()
               .Include(p => p.Product)
               .ProjectTo<ConsumptionModel>(_mapper.ConfigurationProvider)
               .ToList();

            return result;
        }

        
      

    }
    
}
