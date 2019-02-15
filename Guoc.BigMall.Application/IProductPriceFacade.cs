using System.Collections.Generic;

using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Application.Search;

namespace Guoc.BigMall.Application
{
    public interface IProductPriceFacade
    {
        ProductPriceDto GetCurrentPriceByProductId(int storeId, int productId);
        ProductPriceDto GetCurrentPriceByProductCode(int storeId, string productCode);
        List<ProductPriceDto> GetCurrentPriceByProductIds(int storeId, List<int> productIds);
        List<ProductPriceDto> GetCurrentPriceByProductCodes(int storeId, List<string> productCodes);
        /// <summary>
        ///  查询门店商品价格
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<StoreProductPriceDto> QueryStoreProductPrice(Pager page, SearchStoreProductPrice condition);

        /// <summary>
        ///  获取门店商品价格
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        StoreProductPriceDto GetStoreProductPrice(int storeId, int productId);
    }
}