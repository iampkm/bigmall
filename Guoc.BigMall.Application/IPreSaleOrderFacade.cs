using System.Collections.Generic;

using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;

namespace Guoc.BigMall.Application
{
    public interface IPreSaleOrderFacade
    {
        void CreatePreSaleOrder(SaleOrderModel model);
        IEnumerable<SaleOrderDto> GetPreSaleOrderDtos(Pager page, SearchSaleOrder condition);
        IEnumerable<SaleOrderModel> GetPreSaleOrderModels(Pager page, SearchSaleOrder condition);
        SaleOrderModel GetPreSaleOrderModel(SearchSaleOrder condition);
        SaleOrderModel GetPreSaleOrderModelByFJCode(string fjCode);
        SaleOrderModel GetPreSaleOrderModelByCode(string orderCode);
        IEnumerable<SaleOrderModel> GetPreSaleOrders(Pager page, SearchSaleOrder condition);
        SaleOrder GetPreSaleOrder(SearchSaleOrder condition);
        SaleOrder GetPreSaleOrderByCode(string orderCode);
        void UpdatePreSaleOrder(SaleOrderModel model);
        void AuditedPreSaleOrder(SaleOrderAuditedModel condition);
        void AbandonPreSaleOrder(SaleOrderAbandonModel condition);
        void RejectPreSaleOrder(SaleOrderRejectModel condition);
        void OutStock(string preOrderCode);
        void ConvertToSaleOrder(string preOrderCode,string fjCode);
        /// <summary>
        ///  预销售单综合查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        IEnumerable<SaleOrderListDetailDto> GetPreSaleOrderListDetail(Pager page, SearchSaleOrder condition);
    }
}