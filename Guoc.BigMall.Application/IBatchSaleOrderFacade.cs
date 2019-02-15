using System.Collections.Generic;

using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;

namespace Guoc.BigMall.Application
{
    public interface IBatchSaleOrderFacade
    {
        void CreateBatchSaleOrder(SaleOrderModel model);
        IEnumerable<SaleOrderDto> GetBatchSaleOrderDtos(Pager page, SearchSaleOrder condition);
        IEnumerable<SaleOrderModel> GetBatchSaleOrderModels(Pager page, SearchSaleOrder condition);
        SaleOrderModel GetBatchSaleOrderModel(SearchSaleOrder condition);
        SaleOrderModel GetBatchSaleOrderModelByFJCode(string fjCode);
        SaleOrderModel GetBatchSaleOrderModelByCode(string orderCode);
        IEnumerable<SaleOrder> GetBatchSaleOrders(Pager page, SearchSaleOrder condition);
        SaleOrder GetBatchSaleOrder(SearchSaleOrder condition);
        SaleOrder GetBatchSaleOrderByCode(string orderCode);
        void UpdateBatchSaleOrder(SaleOrderModel model);
        void AuditedBatchSaleOrder(SaleOrderAuditedModel condition);
        void AbandonBatchSaleOrder(SaleOrderAbandonModel condition);
        void RejectBatchSaleOrder(SaleOrderRejectModel condition);
        void OutStock(SaleOrderOutStockModel model);

        /// <summary>
        ///  批发销售综合查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        IEnumerable<SaleOrderListDetailDto> GetBatchSaleOrderListDetail(Pager page, SearchSaleOrder condition);

        void PostOrderToSap(string code);
        List<SaleOrderModel> GetPrintList(string ids);
    }
}