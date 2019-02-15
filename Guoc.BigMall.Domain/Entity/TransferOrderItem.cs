using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class TransferOrderItem : BaseEntity
    {
        public int TransferOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ActualShipmentQuantity { get; set; }
        public int ActualReceivedQuantity { get; set; }
        public decimal Price { get; set; }

        public string ProductCode { get; set; }
        public string Unit { get; set; }
        public string SapRow { get; set; }
        public string SNCodes { get; set; }
    }
}
