using System.Collections.Generic;

using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;

namespace Guoc.BigMall.Application
{
    public interface IReturnPreSaleOrderFacade
    {
        void CreateReturnPreSaleOrder(SaleOrderModel model);
        SaleOrderModel GetReturnPreSaleOrderByCode(string code);
        SaleOrderModel GetReturnPreSaleOrderBySaleCode(string saleCode);
        IEnumerable<SaleOrderModel> GetReturnPreSaleOrders(Pager page, SearchSaleOrder condition);
        void InStock(ReturnBatchOrderModel model);
        void AbandonReturnPreSaleOrder(SaleOrderAbandonModel condition);
    }
}