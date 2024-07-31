using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IZavrsniDbContext _context;

         public AuthService(IZavrsniDbContext context)
        {
            _context = context;
        }


    

        public async Task<User> Login(string username, string password)
        {
            return await _context.Users
              .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}
