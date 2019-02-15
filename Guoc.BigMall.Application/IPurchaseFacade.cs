using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IPurchaseFacade
    {
        IEnumerable<PurchaseOrderDetailListDto> GetDetailList(Pager page, SearchPurchaseOrder condition);


        /// <summary>
        /// 查询采购订单列表（包含明细）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<PurchaseOrderDto> GetOrderList(string id);

        PurchaseOrderDto GetReceiveById(int id);
        PurchaseOrderDto GetById(int id);

        void SubmitToSap(PurchaseOrder purchaseOrder, string EditByName, string remark);

        /// <summary>
        ///  入库
        /// </summary>
        /// <param name="model"></param>
        void StockIn(PurchaseOrderModel model);

        void SapStockIn(PurchaseOrder purchaseOrder, List<StoreInventoryHistorySAP> historySapList, ProcessHistory phistory, bool isInStock);

        IEnumerable<PurchaseOrderDetailListDto> GetRefundDetailList(Pager page, SearchPurchaseOrder condition);
        /// <summary>
        /// 作废采购退单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="editBy"></param>
        /// <param name="editor"></param>
        /// <param name="reason"></param>
        void Cancel(int id, int editBy, string editor, string reason);

        /// <summary>
        /// 查询可退商品
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        IEnumerable<ProductDto> GetRefundProduct(Pager page, SearchProduct condition);

        void RefundCreate(PurchaseOrderModel model);
        void RefundEdit(PurchaseOrderModel model);
        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="model"></param>
        void StockOut(PurchaseOrderModel model);


        void ChangeCreate(PurchaseOrderModel model);
        void ChangeEdit(PurchaseOrderModel model);


        void Audit(int id, int editBy, string editor, string reason);
        void Reject(int id, int editBy, string editor, string reason);
        void ChangeCancel(int id, int editBy, string editor, string reason);

        void ChangeStockIn(PurchaseOrderModel model);
        void Close(int id);

        /// <summary>
        /// 初始化导入库存：上线前使用
        /// </summary>
        void InitStockIn(string codes);
    }
}
