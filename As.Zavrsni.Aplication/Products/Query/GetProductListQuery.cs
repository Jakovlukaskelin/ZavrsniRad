using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Products.Model;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Products.Query
{
    public class GetProductListQuery : IRequest<List<ProductsModel>>
    {

    }

    public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, List<ProductsModel>>
    {
        private readonly IZavrsniDbContext _context;
        private readonly IMapper _mapper;

        public GetProductListQueryHandler(IZavrsniDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProductsModel>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
        {
            var productEntities = await this._context.Products
                 .AsNoTracking()
                .ProjectTo<ProductsModel>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

           
            return productEntities;
        }
        
    }
}
