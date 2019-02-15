using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class PurchaseOrderReceive:BaseEntity
    {
        private List<PurchaseOrderReceiveItem> _items;
        public PurchaseOrderReceive() 
        {
            this.CreateOn = DateTime.Now;
            _items = new List<PurchaseOrderReceiveItem>();
        }
        public long Code { get; set; }
        public DateTime CreateOn { get; set; }
        public int CreatedBy { get; set; }
        public int PurchaseOrderId { get; set; }

        public void AddItem(PurchaseOrderReceiveItem item)
        {
            item.PurchaseOrderReceiveId = this.Id;
            if (this._items.Exists(n => n.ProductId == item.ProductId))
            {
                var productLine = this._items.FirstOrDefault(n => n.ProductId == item.ProductId);
                productLine.PurchaseQuality += item.PurchaseQuality;
            }
            else
            {
                this._items.Add(item);
            }
        }

        public void SetItems(List<PurchaseOrderReceiveItem> items)
        {
            this._items = items;
        }
        public virtual IEnumerable<PurchaseOrderReceiveItem> Items
        {
            get
            {
                return _items;
            }
        }


    }
}
