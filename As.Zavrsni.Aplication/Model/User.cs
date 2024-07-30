using As.Zavrsni.Aplication.Interface.Mapping;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Model
{
    public class User : IHaveCustomMapping

    {

        public string username { get; set; } 
        public string password { get; set; }

        public void CreateMappings(Profile configuration)
        {
           configuration.CreateMap<User, User>();
        }
    }
}
