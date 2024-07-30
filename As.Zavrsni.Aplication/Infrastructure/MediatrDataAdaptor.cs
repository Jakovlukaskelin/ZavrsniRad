using As.Zavrsni.Aplication.Common;
using MediatR;
using Newtonsoft.Json;
using Syncfusion.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Infrastructure
{
    public class MediatrDataAdaptor<T, R> : DataAdaptor where T : PagedRequest where R : class
    {
        private IMediator _Mediator { get; set; }

        public MediatrDataAdaptor(IMediator mediator)
        {
            _Mediator = mediator;
        }

        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            T getPageQuery = null;
            if (dm.Params != null && dm.Params.Count > 0)
            {
                getPageQuery = JsonConvert.DeserializeObject<T>(dm.Params.First().Value.ToString());
            }
            else
            {
                getPageQuery = (T)Activator.CreateInstance(typeof(T));
                getPageQuery.Skip = null;
                getPageQuery.Take = null;
                getPageQuery.Sorts = new SortByData[0];
                getPageQuery.Filters = new FilterByData[0];
                getPageQuery.Keywords = null;
            }



            var filters = new List<FilterByData>();

            if (dm.Search != null && dm.Search.Count > 0)
            {
                foreach (var searchTerm in dm.Search)
                {
                    if (searchTerm.Fields.Count == 1 && searchTerm.Fields[0] == "EVERYTHING")
                    {
                        getPageQuery.Keywords = searchTerm.Key;
                    }
                    else
                    {
                        foreach (var columnName in searchTerm.Fields)
                        {
                            filters.Add(new FilterByData { ColumnName = columnName, Keywords = searchTerm.Key, });
                        }
                    }
                }
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                getPageQuery.Sorts = dm.Sorted.Select(s =>
                {
                    return new SortByData { ColumnName = s.Name, SortDirection = s.Direction, };
                }).ToArray();
            }
            if (dm.Where != null && dm.Where.Count > 0)
            {
            }

            if (dm.RequiresCounts)
            {
                getPageQuery.Skip = dm.Skip > 0 ? dm.Skip : null;
                getPageQuery.Take = dm.Take > 0 ? dm.Take : 100;
            }

            getPageQuery.Filters = filters;

            var page = await _Mediator.Send(getPageQuery) as PagedResponse<R>;

            if (dm.RequiresCounts)
            {
                return new Syncfusion.Blazor.Data.DataResult { Result = page.Items.ToList(), Count = page.Count };
            }
            else
            {
                return page.Items;
            }
        }
    }

}
