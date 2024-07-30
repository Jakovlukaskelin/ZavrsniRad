using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Infrastructure.AutoMapper
{
    public class AutoMapperProfile : Profile

    {
        public AutoMapperProfile()
        {
            LoadStandardMappings();
            LoadCustomMappings();
            LoadConverters();

            this.CreateMap<Guid?, Guid>().ConvertUsing((src, dest) => src ?? dest);
        }

        private void LoadConverters()
        {

        }

        private void LoadStandardMappings()
        {
            var mapsFrom = MapperProfileHelper.LoadStandardMappings(Assembly.GetExecutingAssembly());

            foreach (var map in mapsFrom)
            {
                CreateMap(map.Source, map.Destination).ReverseMap();
            }
        }

        private void LoadCustomMappings()
        {
            var mapsFrom = MapperProfileHelper.LoadCustomMappings(Assembly.GetExecutingAssembly());

            foreach (var map in mapsFrom)
            {
                map.CreateMappings(this);
            }
        }
    }
}

