using As.Zavrsni.Aplication.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Model.Query
{
    public class QueryUser:IRequest<List<UserModel>>
    {
    }

    public class GetUsersFromDbQuery : IRequestHandler<QueryUser, List<UserModel>>
    {
        private readonly IZavrsniDbContext _context;
        private readonly IMapper _mapper;

        public GetUsersFromDbQuery(IZavrsniDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserModel>> Handle(QueryUser request, CancellationToken cancellationToken)
        {
            var userEntities = await _context.Users
                .Where(x => x.UserId == x.UserId)
                .ProjectTo<UserModel>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            
            return userEntities;
        }
    }

}
