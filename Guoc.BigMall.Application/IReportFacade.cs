using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IReportFacade
    {
        IEnumerable<PurchaseSaleInventorySummaryDto> GetPageList(Pager page, SearchPurchaseSaleInventorySummary condition);
        IEnumerable<SaleOrderItemSummaryDto> GetSaleOrderItemList(Pager page, SearchSaleOrderItemSummary condition);
        IEnumerable<SaleOrderSummaryDto> GetSaleOrderSummaryList(Pager page, SearchSaleOrderSummary condition);

        IEnumerable<OrderProfitDto> GetOrderProfitList(Pager page, SearchProfit condition);
    }
}
