using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Common
{
    public class PagedRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<SortByData> Sorts { get; set; }
        public IEnumerable<FilterByData> Filters { get; set; }
        public string Keywords { get; set; }

        public PagedRequest()
        {
            Skip = null;
            Take = null;
            Sorts = new List<SortByData>();
            Filters = new List<FilterByData>();
        }
    }
    public class FilterByData
    {
        public string ColumnName { get; set; }
        public string Keywords { get; set; }
    }

    public class SortByData
    {
        public string ColumnName { get; set; }
        public string SortDirection { get; set; }
    }
    // Helper class to replace parameter expressions in a tree
    public class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}
