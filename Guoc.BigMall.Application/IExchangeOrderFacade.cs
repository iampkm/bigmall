using System.Collections.Generic;

using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;

namespace Guoc.BigMall.Application
{
    public interface IExchangeOrderFacade
    {
        void CreateExchangeOrder(SaleOrderModel model);
        IEnumerable<SaleOrderDto> GetExchangeOrderDtos(Pager page, SearchSaleOrder condition);
        IEnumerable<SaleOrderModel> GetExchangeOrderModels(Pager page, SearchSaleOrder condition);
        SaleOrderModel GetExchangeOrderModel(SearchSaleOrder condition);
        SaleOrderModel GetExchangeOrderModelByFJCode(string fjCode);
        SaleOrderModel GetExchangeOrderModelByCode(string exchangeOrderCode);
        IEnumerable<SaleOrder> GetExchangeOrders(Pager page, SearchSaleOrder condition);
        SaleOrder GetExchangeOrder(SearchSaleOrder condition);
        SaleOrder GetExchangeOrderByCode(string exchangeOrderCode);
        void UpdateExchangeOrder(SaleOrderModel model);
        void AuditedExchangeOrder(SaleOrderAuditedModel condition);
        void AbandonExchangeOrder(SaleOrderAbandonModel condition);
        void RejectExchangeOrder(SaleOrderRejectModel condition);
        void InOutStock(string exchangeOrderCode);
    }
}