using System.Collections.Generic;

using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;

namespace Guoc.BigMall.Application
{
    public interface ISaleOrderFacade
    {
        void CreateSaleOrder(SaleOrderModel model);
        IEnumerable<SaleOrderDto> GetSaleOrderDtos(Pager page, SearchSaleOrder condition);
        IEnumerable<SaleOrderModel> GetSaleOrderModels(Pager page, SearchSaleOrder condition);
        SaleOrderModel GetSaleOrderModel(SearchSaleOrder condition);
        SaleOrderModel GetSaleOrderModelByFJCode(string fjCode);
        SaleOrderModel GetSaleOrderModelByCode(string orderCode);
        IEnumerable<SaleOrder> GetSaleOrders(Pager page, SearchSaleOrder condition);
        SaleOrder GetSaleOrder(SearchSaleOrder condition);
        SaleOrder GetSaleOrderByCode(string orderCode);
        void UpdateSaleOrder(SaleOrderModel model);
        void AuditedSaleOrder(SaleOrderAuditedModel condition);
        void AbandonSaleOrder(SaleOrderAbandonModel condition);
        void RejectSaleOrder(SaleOrderRejectModel condition);
        void OutStock(string orderCode);
        IEnumerable<SaleOrderModel> GetSaleOrderItems(Pager page, SearchSaleOrder condition);

        /// <summary>
        /// 销售综合查询列表明细
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        IEnumerable<SaleOrderListDetailDto> GetSaleOrderListDetail(Pager page, SearchSaleOrder condition);
    }
}