using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferOrderItemDto
    {
        public int Id { get; set; }

        public int TransferOrderId { get; set; }

        public int ProductId { get; set; }

        public bool HasSNCode { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        public string Spec { get; set; }
        public string Unit { get; set; }

        public int Quantity { get; set; }
        public int ActualShipmentQuantity { get; set; }
        public int ActualReceivedQuantity { get; set; }

        public int InventoryQuantity { get; set; }

        public decimal Price { get; set; }

        public decimal Amount { get { return this.Price * this.Quantity; } }

        public string SapRow { get; set; }
        public string SNCodes { get; set; }
        public string[] StockOutSNCodes { get { return string.IsNullOrEmpty(this.SNCodes) ? new string[0] : this.SNCodes.Split(','); } }
    }
}
