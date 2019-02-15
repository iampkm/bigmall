using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IApiFacade
    {
        string ClosePurchase(string code);
        string PurchaseOrderInOrOut(string code);
        string PurchaseOrderToSap(string code);
        string TransferOrdeToSap(string code);
        string TransferOrderInOrOut(string code);
        string SaleOrdeToSap(string code);
        string SaleOrderOutToSap(string code);
        string SaleOrderInToSap(string code);
        string PreOrderReturnInStock(string code);
        string PreConvertOrder(string code);
        string AbandonSaleOrder(string code);

        void initInventory(string excel, string file);
        string CreateROWithNoSourceSO(string storeCode, string snCode, string productCode, int quantity, decimal realPrice);
        void ROInStockWithNoSourceSO(string returnOrderCode, string supplierCode, decimal costPrice);
    }
}
