using System.Collections.Generic;

using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;

namespace Guoc.BigMall.Application
{
    public interface IReturnBatchSaleOrderFacade
    {
        void CreateReturnBatchSaleOrder(SaleOrderModel model);
        SaleOrderModel GetReturnBatchSaleOrderByCode(string returnCode);
        SaleOrderModel GetReturnBatchSaleOrderBySaleCode(string saleCode);
        IEnumerable<SaleOrderModel> GetReturnBatchSaleOrders(Pager page, SearchSaleOrder condition);
        void InStock(ReturnBatchOrderModel model);
    }
}