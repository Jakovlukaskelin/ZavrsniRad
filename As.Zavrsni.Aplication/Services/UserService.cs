using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Services
{
    public class UserService
    {
        public string LoggedInUsername { get; private set; }

        public void SetLoggedInUsername(string username)
        {
            LoggedInUsername = username;
        }
    }
}
