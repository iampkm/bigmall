using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferItemDetailCreateModel
    {
        public int TransferOrderId { get; set; }
        public int TransferOrderItemId { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ActualShipmentQuantity { get; set; }
        public int ActualReceivedQuantity { get; set; }
        public decimal ContractPrice { get; set; }
        public decimal Price { get; set; }
        public long BatchNo { get; set; }
        public string SNCodes { get; set; }
    }
}
