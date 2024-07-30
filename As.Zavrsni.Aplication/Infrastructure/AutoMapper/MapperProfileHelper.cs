using As.Zavrsni.Aplication.Interface.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Infrastructure.AutoMapper
{
    public sealed class Map
    {
        public Type Source { get; set; }
        public Type Destination { get; set; }
    }
    public class MapperProfileHelper
    {
        public static IList<Map> LoadStandardMappings(Assembly rootAssembly)
        {
            var types = rootAssembly.GetExportedTypes();

            var mapsFrom = types
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => t.GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .Where(i => i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                        .Any())
                .Select(t => new Map
                {
                    Source = t.GetInterfaces().First().GetGenericArguments().First(),
                    Destination = t
                })
                .ToList();

            return mapsFrom;
        }

        public static IList<IHaveCustomMapping> LoadCustomMappings(Assembly rootAssembly)
        {
            var types = rootAssembly.GetExportedTypes();

            var mapsFrom = types
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => typeof(IHaveCustomMapping).IsAssignableFrom(t))
                .Select(t => (IHaveCustomMapping)Activator.CreateInstance(t))
                .ToList();

            return mapsFrom;
        }
    }
}

