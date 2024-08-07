﻿using As.Zavrsni.Aplication.Interface.Mapping;
using As.Zavrsni.Domain.Entites;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Model
{
    public class Users : IHaveCustomMapping

    {

        public int UserId { get; set; }
        public string username { get; set; } 
        public string password { get; set; }
        public int? RoleId { get; set; }

        public void CreateMappings(Profile configuration)
        {
           configuration.CreateMap<User, Users>();
        }
    }
}
