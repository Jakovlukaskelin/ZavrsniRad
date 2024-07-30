using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Interface
{
    public interface IComponentRendererService
    {
        public Task<string> RenderComponent<T>(Dictionary<string, object?> dictionary) where T : IComponent;
    }
}
