using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace As.Zavrsni.Aplication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IZavrsniDbContext _context;

         public AuthService(IZavrsniDbContext context)
        {
            _context = context;
        }




        public async Task<User?> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return user;
                }
               
                else if (user.Password == password)
                {
                  
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    await _context.SaveChangesAsync();
                    return user;
                }
            }
            return null;
        }

    }
}
