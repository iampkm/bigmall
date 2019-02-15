using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.Objects;
namespace Guoc.BigMall.Domain
{
    /// <summary>
    /// SAP系统提供的服务。
    /// </summary>
    public interface ISAPService
    {
        /// <summary>
        /// 采购订单在商玛特上做关单后，传到SAP，SAP做关单处理（订单创建日期+7天）。使用范围：采购订单。
        /// </summary>
        /// <param name="sapCode">采购单号</param>
        void ClosePurchaseOrder(string sapCode);

        /// <summary>
        /// 采购在SMT系统制作采购订单，SMT把采购订单发至SAP，在SAP系统生成SAP采购订单。
        /// 适应业务流程范围及流程步骤：门店间调拨流程；门店向总仓申请调拨流程；采购换机流程；采购退货流程（门店发货）。
        /// </summary>
        /// <param name="order"></param>
        void SubmitPurchaseOrder(Order order);

        /// <summary>
        /// SMT回传SAP接口，在SMT系统扫码入库后，SMT需把入库信息回传SAP，SAP在系统生成收货单。
        /// 适应业务流程范围及流程步骤：总仓订货流程；直送订货流程；采购换机流程；采购退货流程（门店发货）；采购退货流程（仓店发货）;分公司订货流程。
        /// </summary>
        /// <param name="poReceive"></param>
        void PurchaseOrderInStock(POReceive poReceive);

        /// <summary>
        /// 调拨出库SMT发至SAP接口，在SMT完成扫码发货后，SMT需把出货信息发至SAP，SAP在系统生成发货单。资源分配流程；门店间调拨流程；门店向总仓申请调拨流程。
        /// </summary>
        /// <param name="transferStockOut"></param>
        void TransferOrderOutStock(TransferStockOut transferStockOut);

        /// <summary>
        /// SMT把调拨收货单发至SAP，在SAP系统生成收货单。适应业务流程范围及流程步骤：资源分配流程；门店间调拨流程；门店向总仓申请调拨流程。
        /// </summary>
        /// <param name="transferStockIn"></param>
        void TransferOrderInStock(TransferStockIn transferStockIn);

        /// <summary>
        /// 接口用于接收商玛特盘点差异过账信息。在商玛特平台上进行盘点，盘点完成后，通过该接口将盘点差异上传到SAP，并在SAP系统进行盘点差异过账。
        /// </summary>
        void SubmitInventoryDifference(StocktakingPlan entity);

        /// <summary>
        /// 该接口用于接收商玛特发货过账数据。商玛特平台接收到SAP销售订单生成成功的标识后，可在商玛特平台上进行发货过账。
        /// 商玛特发货成功，用过该接口将发货数据发至SAP，SAP调取该接口数据，在SAP上发货过账。
        /// 目前有以下流程需用到该接口：零售销售流程、零售换机流程、零售退货流程、预销售流程、批发销售、批发退货。
        /// </summary>
        void SubmitDelivery(List<StoreInventoryHistorySAP> entitys);

        /// <summary>
        /// 该接口用于接收商玛特预销售关闭单据信息。预销售退货，在商玛特平台做原单发货冲销后，通过该接口传至SAP。
        /// 再在SAP中找到原单，对已发货的原单冲销退货，并关闭原单。对未发货的原单，直接做关闭处理。
        /// 目前有以下流程需用到该接口：预销售退货流程。
        /// </summary>
        void ClosePreSaleOrder(SaleOrder presaleOrder);

        /// <summary>
        ///  预销售订单转正
        /// </summary>
        /// <param name="order">预销售订单</param>
        void ConvertPreSaleOrder(SaleOrder order);

        /// <summary>
        /// SAP接口-商玛特预销售关单上传.目前有以下流程需用到该接口：零售销售流程、零售换机流程、零售退货流程、预销售流程。(批发订单不能调用)
        /// </summary>
        void SubmitSaleOrder(SaleOrder order);
    }
}
