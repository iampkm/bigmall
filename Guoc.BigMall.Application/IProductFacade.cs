using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IProductFacade
    {
        IEnumerable<ProductDto> GetList(Pager page, SearchProduct search);
        IEnumerable<ProductDto> GetSNcodeList(Pager page, SearchProduct search);
        IEnumerable<ProductDto> LoadStoreProduct(Pager page, SearchProduct search);
        IEnumerable<ProductDto> LoadProduct(SearchProduct search);
        IEnumerable<ProductDto> LoadSupplierProduct(Pager page, SearchProduct search);
        ProductDto LoadProductById(int storeId, int productId);

        /// <summary>
        /// 查询当前门店下的串码商品
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="snCode"></param>       
        /// <returns></returns>
        ProductDto QueryProduct(int storeId, string snCode);
    }
}
