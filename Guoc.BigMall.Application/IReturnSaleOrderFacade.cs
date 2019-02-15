using System.Collections.Generic;

using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;

namespace Guoc.BigMall.Application
{
    public interface IReturnSaleOrderFacade
    {
        void CreateReturnSaleOrder(SaleOrderModel model);
        SaleOrderModel GetReturnSaleOrderByCode(string code);
        SaleOrderModel GetReturnSaleOrderBySaleCode(string saleCode);
        IEnumerable<SaleOrderModel> GetReturnSaleOrders(Pager page, SearchSaleOrder condition);
        void InStock(ReturnBatchOrderModel model);
    }
}