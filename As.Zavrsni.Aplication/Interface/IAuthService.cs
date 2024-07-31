

using As.Zavrsni.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Interface
{
    public interface IAuthService
    {
        Task<User> Login(string username, string password);
    }
}
