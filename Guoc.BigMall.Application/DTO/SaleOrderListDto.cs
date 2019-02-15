using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class SaleOrderListDto
    {
        public string MasterId { get; set; }
        public string Code { get; set; }
        public string OrderType { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public SaleOrderStatus Status { get; set; }

        public string SaleOrderStatus {
            get
            {
                return Status.Description();
            }
        }

        public string Remark { get; set; }
        public string Name { get; set; }
        public string ItemId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public decimal RealPrice { get; set; }
        public string SNCode { get; set; }
        public string FJCode { get; set; }
    }
}