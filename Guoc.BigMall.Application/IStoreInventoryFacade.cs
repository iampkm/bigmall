using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IStoreInventoryFacade
    {
        IEnumerable<StoreInventoryQueryDto> GetPageList(Pager page, SearchStoreInventory condition);
        IEnumerable<StoreInventoryHistoryQueryDto> GetPageHistoryList(Pager page, SearchStoreInventoryHistory condition);
        IEnumerable<StoreInventoryBatchQueryDto> GetPageBatchList(Pager page, SearchStoreInventoryBatch condition);

        IEnumerable<SNCodeInventoryDto> GetSNCodeProduct(SearchStoreInventory condition);
        //void StockIn(PurchaseOrder entity);

        void JudgeSnCodes(List<PurchaseOrderItem> receiveList);

        bool LockProductInventory(int storeId, int productId, int qty);

        StoreInventory GetInventory(int storeId, int productId);
        List<StoreInventory> GetInventory(int storeId, IEnumerable<int> productIds);
        StoreInventoryBatch GetInventoryBatch(int storeId, string snCode, int productId);
        List<StoreInventoryBatch> GetInventoryBatch(int storeId, IEnumerable<int> productIds);
        List<StoreInventoryBatch> GetInventoryBatch(int storeId, IEnumerable<string> snCodes, IEnumerable<int> productIds);
        List<StoreInventoryBatchDto> GetStockOutInventoryBatch(int storeId, int billId, BillIdentity billType, int productId);
        StoreInventoryBatch GetLastInventoryBatchWithNoSNCode(int storeId, int productId);
        StoreInventoryHistory GetHistoryWithSNCode(int storeId, int billId, BillIdentity billType, int productId, string snCode);
        void JudgeSnCodeProduct(List<PurchaseOrderItem> receiveList);
        StoreInventoryResult OutStock(IDBContext dbContext, StockOutModel model);
        StoreInventoryResult InStock(IDBContext dbContext, StockInModel model, bool generateSapHistory = true);
        void SaveStoreInventoryHistorySap(IDBContext dbContext, StoreInventoryResult storeInventoryChangedResult, string historyCode = null);
        decimal CalculatedAveragePrice(decimal currentAvgCostPrice, int currentQuantity, decimal price, int quantity);
        IEnumerable<InventorySummaryDto> GetInventorySummary(Pager page, SearchInventorySummary condition);
    }
}
