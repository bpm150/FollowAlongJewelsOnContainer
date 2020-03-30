using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogApi.ViewModel
{
    public class PaginatedItemsViewModel<TEntity>
        where TEntity : class
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public long ItemCount { get; set; } // could be many

        public IEnumerable<TEntity> Data { get; set; }
        // My intuition was List<> since we wrote this class
        // to facilitate passing back data about a List<CatalogItem>

    }
}
