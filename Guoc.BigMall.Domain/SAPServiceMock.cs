using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain
{
    public class SAPServiceMock : ISAPService
    {

        public void ClosePurchaseOrder(string sapCode)
        {

        }

        public void SubmitPurchaseOrder(Order order)
        {

        }

        public void PurchaseOrderInStock(POReceive poReceive)
        {

        }

        public void SubmitInventoryDifference(Entity.StocktakingPlan entity)
        {

        }

        public void SaleDelivery()
        {

        }

        public void ClosePreSaleOrder(SaleOrder presaleOrder)
        {

        }


        public void TransferOrderOutStock(TransferStockOut transferStockOut)
        {

        }

        public void TransferOrderInStock(TransferStockIn transferStockIn)
        {

        }


        public void SubmitSaleOrder(Entity.SaleOrder order)
        {

        }


        public void SubmitDelivery(List<Entity.StoreInventoryHistorySAP> entitys)
        {

        }

        public void ConvertPreSaleOrder(Entity.SaleOrder order)
        {

        }
    }
}
