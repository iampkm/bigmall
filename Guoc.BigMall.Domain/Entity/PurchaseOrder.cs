using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class PurchaseOrder : BaseEntity
    {

        private List<PurchaseOrderItem> _items;
        public PurchaseOrder()
        {
            this.CreatedOn = DateTime.Now;
            this.UpdatedOn = DateTime.Now;
            _items = new List<PurchaseOrderItem>();
            this.Status = CBPurchaseOrderStatus.Create;
        }


        public string Code { get; set; }

        public string SapCode { get; set; }
        public int SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public CBPurchaseOrderStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int UpdatedBy { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 物流/收货验证码
        /// </summary>

        /// <summary>
        /// 单据类型
        /// </summary>
        public PurchaseOrderBillType BillType { get; set; }
        public PurchaseOrderType OrderType { get; set; }
        public bool IsPushSap { get; set; }
        public virtual IEnumerable<PurchaseOrderItem> Items
        {
            get
            {
                return _items;
            }
        }


        public void AddItems(List<PurchaseOrderItem> items)
        {
            foreach (var item in items)
            {
                item.PurchaseOrderId = this.Id;
                if (this._items.Exists(n => n.ProductId == item.ProductId))
                {
                    var productLine = this._items.Where(n => n.ProductId == item.ProductId).FirstOrDefault();
                    productLine.Quantity += item.Quantity;

                }
                else
                {
                    this._items.Add(item);
                }
            }
        }


        public void SetItems(List<PurchaseOrderItem> items)
        {
            this._items = items;
        }


        public void AddItem(PurchaseOrderItem item)
        {
            item.PurchaseOrderId = this.Id;
            if (this._items.Exists(n => n.ProductId == item.ProductId))
            {
                var productLine = this._items.FirstOrDefault(n => n.ProductId == item.ProductId);
                productLine.Quantity += item.Quantity;

            }
            else
            {
                this._items.Add(item);
            }
        }
        public void AddItemsForChange(List<PurchaseOrderItem> items)
        {
            foreach (var item in items)
            {
                item.PurchaseOrderId = this.Id;
                if (this._items.Exists(n => n.ProductId == item.ProductId && n.Quantity < 0))
                {
                    var productLine = this._items.FirstOrDefault(n => n.ProductId == item.ProductId && n.Quantity < 0);
                    productLine.Quantity += item.Quantity;
                }
                else
                {
                    this._items.Add(item);
                }
            }
        }
        public void Clear()
        {
            this._items.Clear();
        }



        private void Edit(int editBy)
        {
            this.UpdatedBy = editBy;
            this.UpdatedOn = DateTime.Now;
        }

        public void Finished(int editBy)
        {
            if (this.Status != CBPurchaseOrderStatus.Audited)
            {
                this.Status = CBPurchaseOrderStatus.Finished;

                this.Edit(editBy);
            }
            else
            {
                throw new Exception("订单非已发货状态");
            }
        }


        public void Cancel(int editBy)
        {

            //采购单/退单 /换单 可作废状态为：初始，已审。 已出库就不能作废了
            if (this.Status == CBPurchaseOrderStatus.Create || this.Status == CBPurchaseOrderStatus.Audited || this.Status == CBPurchaseOrderStatus.Reject)
            {
                this.Status = CBPurchaseOrderStatus.Cancel;
            }
            else
            {
                throw new Exception("已出库前的采购退单可作废");
            }

            Edit(editBy);
        }



        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="editBy"></param>
        public void StockIn(int editBy)
        {
            if (this.Status != CBPurchaseOrderStatus.Audited)
            {
                throw new Exception("已审核的商品才能进行操作");
            }
            //this.Status = CBPurchaseOrderStatus.Finished;
            this.Edit(editBy);
        }

        public BillIdentity GetBillIdentity()
        {
            switch (this.OrderType)
            {
                case PurchaseOrderType.PurchaseOrder:
                    return this.BillType == PurchaseOrderBillType.StockOrder ? BillIdentity.StockPurchaseOrder : BillIdentity.StorePurchaseOrder;
                case PurchaseOrderType.PurchaseReturn:
                    return this.BillType == PurchaseOrderBillType.StockOrder ? BillIdentity.StockPurchaseRefundOrder : BillIdentity.StorePurchaseRefundOrder;
                case PurchaseOrderType.PurchaseChange:
                    return BillIdentity.StorePurchaseChange;
                default:
                    throw new Exception("无效的采购单类型！");
            }
        }

        /// <summary>
        /// 更新收货明细中的 ，数量，生成日期，保质期
        /// </summary>
        /// <param name="items"></param>
        public bool UpdateReceivedGoodsItems(List<PurchaseOrderItem> items)
        {
            Dictionary<int, PurchaseOrderItem> dic = new Dictionary<int, PurchaseOrderItem>();
            items.ForEach(n => dic.Add(n.Id, n));
            var allReceive = true;
            foreach (var item in this._items)
            {
                if (dic.ContainsKey(item.Id))
                {
                    if (item.Quantity == item.ActualQuantity)
                    {
                        items.Remove(dic[item.Id]);
                        continue;
                    }
                    item.ActualQuantity += dic[item.Id].SNQuantity;
                    item.SNQuantity = dic[item.Id].SNQuantity;
                    item.SNCodes = dic[item.Id].SNCodes;
                    item.IsSnCode = dic[item.Id].IsSnCode;
                    if (item.ActualQuantity > item.Quantity) { item.ActualQuantity = item.Quantity; }

                }
                if (allReceive && item.Quantity != item.ActualQuantity)
                {
                    allReceive = false;
                }

            }
            return allReceive;

        }
        /// <summary>
        /// 更新退货明细中，应退和实退保持一致
        /// </summary>
        /// <param name="items"></param>
        public void UpdateReturnedGoodsItems(List<PurchaseOrderItem> items)
        {
            Dictionary<int, PurchaseOrderItem> dic = new Dictionary<int, PurchaseOrderItem>();
            items.ForEach(n => dic.Add(n.Id, n));
            foreach (var item in this._items)
            {
                if (dic.ContainsKey(item.Id))
                {
                    if (Math.Abs(item.Quantity) != Math.Abs(dic[item.Id].SNQuantity))
                    {
                        throw new FriendlyException(string.Format("商品{0}出库数不正确，应全部出库！", item.ProductCode));
                    }
                    item.ActualQuantity = item.Quantity;
                    item.IsSnCode = dic[item.Id].IsSnCode;
                    item.SNCodes = dic[item.Id].SNCodes;
                }

            }
        }

        /// <summary>
        /// 更新换货明细中
        /// </summary>
        /// <param name="items"></param>
        public void UpdateChangedGoodsItems(List<PurchaseOrderItem> items)
        {
            Dictionary<int, PurchaseOrderItem> dic = new Dictionary<int, PurchaseOrderItem>();
            items.ForEach(n => dic.Add(n.Id, n));
            foreach (var item in this._items)
            {
                if (dic.ContainsKey(item.Id))
                {
                    item.ActualQuantity += dic[item.Id].SNQuantity;
                    item.IsSnCode = dic[item.Id].IsSnCode;
                    item.SNCodes = dic[item.Id].SNCodes;
                    item.SNQuantity = dic[item.Id].SNQuantity;
                }

            }
        }
        /// <summary>
        /// 设置实收数,默认实发 = 申请数
        /// </summary>
        /// <param name="items"></param>
        //public void SetActualQuantity()
        //{
        //    this._items.ForEach(n => n.ActualShipQuantity = n.Quantity);
        //}

        public decimal GetOrderAmount()
        {
            return this.Items.Sum(n => n.CostPrice * n.Quantity);
        }

        /// <summary>
        ///  SAP订单类型（ZP01商玛特采购订单、ZP02商玛特采购退货、ZSM1商玛特采购换货ZUBA商玛特调拨单）
        /// </summary>
        /// <param name="sapOrderType"></param>
        public void SetOrderTypeBySAPType(string sapOrderType)
        {
            if (sapOrderType == "ZP01")
            {
                this.OrderType = PurchaseOrderType.PurchaseOrder;
            }
            else if (sapOrderType == "ZP02")
            {
                this.OrderType = PurchaseOrderType.PurchaseReturn;
            }
            else if (sapOrderType == "ZSM1")
            {
                this.OrderType = PurchaseOrderType.PurchaseChange;
            }
            else
            {
                throw new FriendlyException("分公司采购类型(sap 订单类型：ZUBA)暂未开通!");
            }
        }
        /// <summary>
        /// SAP已审，商码特等待 出库
        /// </summary>
        public void SapAudited()
        {
            this.Status = CBPurchaseOrderStatus.Audited;
            this.UpdatedOn = DateTime.Now;
            this.UpdatedBy = 1;

        }
        /// <summary>
        ///  设置待发货，如果SAP ,FRGKE 审批标识 = X ，默认创建时为已审
        /// </summary>
        /// <param name="sapAudited"></param>
        public void SetWaitShipping(string sapAudited)
        {
            //if (string.IsNullOrEmpty(sapAudited)) {
            //    this.Status = CBPurchaseOrderStatus.Create;
            //}
            //if (sapAudited.ToUpper() == "X")
            //{
            //    this.Status = CBPurchaseOrderStatus.Audited;
            //}
            this.Status = CBPurchaseOrderStatus.Audited;
        }
    }
}
