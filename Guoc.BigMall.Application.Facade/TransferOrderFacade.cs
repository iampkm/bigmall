using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.Utils;
using Guoc.BigMall.Application.Configuration;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.Objects;
using Guoc.BigMall.Domain.Service;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.Facade
{
    public class TransferOrderFacade : ITransferOrderFacade
    {
        IDBContext _db;
        IContextFacade _context;
        IStoreInventoryFacade _storeInventoryFacade;
        ISAPService _sapService;
        BillSequenceService _sequenceService;
        ProductPurchasePriceService _purchasePriceService;
        public TransferOrderFacade(IDBContext dbContext, IContextFacade context, IStoreInventoryFacade storeInventoryFacade, ISAPService sapService, BillSequenceService sequenceService, ProductPurchasePriceService purchasePriceService)
        {
            _db = dbContext;
            _context = context;
            _storeInventoryFacade = storeInventoryFacade;
            _sapService = sapService;
            _sequenceService = sequenceService;
            _purchasePriceService = purchasePriceService;
        }

        public List<TransferOrderDto> GetPageList(Pager page, TransferOrderSearch condition)
        {
            var where = "";
            dynamic param = new ExpandoObject();
            if (condition.Code.NotNullOrEmpty())
            {
                param.Code = condition.Code;
                where += " AND r.Code = @Code";
            }

            if (condition.Type.NotNull())
            {
                param.Type = condition.Type.Value();
                where += " AND r.Type = @Type";
            }

            if (condition.Status.NotNull())
            {
                param.Status = condition.Status.Value();
                where += " AND r.Status = @Status";
            }

            if (condition.FromStoreId.NotNullOrEmpty())
            {
                param.FromStoreId = condition.FromStoreId.Split(',');
                where += " AND r.FromStoreId IN @FromStoreId";
            }

            if (condition.ToStoreId.NotNullOrEmpty())
            {
                param.ToStoreId = condition.ToStoreId.Split(',');
                where += " AND r.ToStoreId IN @ToStoreId";
            }

            //未指定门店查询时，过滤掉与当前账户的门店无关的调拨单
            var canViewStores = _context.CurrentAccount.StoreArray;
            if (condition.FromStoreId.IsNullOrEmpty() && condition.ToStoreId.IsNullOrEmpty() && canViewStores.Length > 0)
            {
                param.FromStoreId = canViewStores;
                param.ToStoreId = canViewStores;
                where += " AND (r.FromStoreId IN @FromStoreId OR r.ToStoreId IN @ToStoreId)";
            }

            if (condition.BatchNo.NotNull())
            {
                param.BatchNo = condition.BatchNo;
                where += " AND r.BatchNo = @BatchNo";
            }
            if (condition.SAPCode.IsNotEmpty())
            {
                where += " AND r.SAPCode = @SAPCode";
                param.SAPCode = condition.SAPCode;
            }
            if (condition.CreateOnFrom.HasValue)
            {
                where += " AND r.CreatedOn >= @CreateOnFrom ";
                param.CreateOnFrom = condition.CreateOnFrom.Value;
            }

            if (condition.CreateOnTo.HasValue)
            {
                where += " AND r.CreatedOn < @CreateOnTo ";
                param.CreateOnTo = condition.CreateOnTo.Value.AddDays(1);
            }

            if (condition.CategoryCode.IsNotEmpty())
            {
                where += " AND c.Code LIKE @CategoryCode ";
                param.CategoryCode = string.Format("{0}%", condition.CategoryCode);
            }

            if (condition.BrandCode.IsNotEmpty())
            {
                where += " AND b.Code = @BrandCode ";
                param.BrandCode = condition.BrandCode;
            }

            if (condition.ProductCode.IsNotEmpty())
            {
                where += " AND i.ProductCode LIKE @ProductCode ";
                param.ProductCode = string.Format("%{0}%", condition.ProductCode);
            }

            if (condition.ProductName.IsNotEmpty())
            {
                where += " AND i.ProductName LIKE @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
            }
            if (condition.ProductIds.IsNotEmpty())
            {
                where += "  AND P.ID IN @productIds ";
                param.productIds = condition.ProductIds.Split(',').ToIntArray();
            }
            if (condition.SNCode.IsNotEmpty())
            {
                where += " AND i.SNCode like @SNCode ";
                param.SNCode = string.Format("%{0}%", condition.SNCode);
            }
            if (condition.BrandIds.IsNotEmpty())
            {
                where += "  AND b.ID IN @BrandIds ";
                param.BrandIds = condition.BrandIds.Split(',').ToIntArray();
            }

            if (condition.IsPushSap.HasValue)
            {
                param.IsPushSap = condition.IsPushSap.Value;
                where += " AND r.IsPushSap = @IsPushSap";
            }

            var fields = @" distinct 
                r.Id ,
                r.Code ,
                r.SapCode ,
                r.Type ,
                r.FromStoreId ,
                fs.Code AS FromStoreCode ,
                r.FromStoreName ,
                r.ToStoreName ,
                r.ToStoreId ,
                ts.Code AS ToStoreCode ,
                r.BatchNo ,
                r.Status ,
                r.Remark ,
                r.CreatedOn ,
                r.CreatedBy ,
                r.CreatedByName ,
                r.UpdatedOn ,
                r.UpdatedBy ,
                r.UpdatedByName,r.IsPushSap ";
            var basicSql = @"
                SELECT  {0}
                FROM    TransferOrder r
                        INNER JOIN TransferOrderItem i on r.Id= i.TransferOrderId 
                        INNER JOIN Store fs ON r.FromStoreId = fs.Id
                        INNER JOIN Store ts ON r.ToStoreId = ts.Id
                        left join product p on p.Id = i.ProductId
		                left join Category c on c.Id = p.CategoryId
		                left join Brand b on b.Id = p.BrandId
                WHERE   1 = 1 " + where;
            var dataSql = basicSql.FormatWith(fields);

            if (page.toExcel)
            {
                return _db.DataBase.Query<TransferOrderDto>(dataSql, param);
            }

            var countSql = basicSql.FormatWith("COUNT(DISTINCT r.Id)");
            var pageSql = "{0} ORDER BY r.Id DESC OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY".FormatWith(dataSql, (page.PageIndex - 1) * page.PageSize, page.PageSize);

            page.Total = _db.DataBase.ExecuteScalar<int>(countSql, param);
            var data = _db.DataBase.Query<TransferOrderDto>(pageSql, param);
            return data;
        }

        public List<TransferOrderDetailDto> GetTransferOrderDetailList(Pager page, TransferOrderSearch condition)
        {
            var where = "";
            dynamic param = new ExpandoObject();
            if (condition.Code.NotNullOrEmpty())
            {
                param.Code = condition.Code;
                where += " AND r.Code = @Code";
            }

            if (condition.Type.NotNull())
            {
                param.Type = condition.Type.Value();
                where += " AND r.Type = @Type";
            }

            if (condition.Status.NotNull())
            {
                param.Status = condition.Status.Value();
                where += " AND r.Status = @Status";
            }

            if (condition.FromStoreId.NotNullOrEmpty())
            {
                param.FromStoreId = condition.FromStoreId.Split(',');
                where += " AND r.FromStoreId IN @FromStoreId";
            }

            if (condition.ToStoreId.NotNullOrEmpty())
            {
                param.ToStoreId = condition.ToStoreId.Split(',');
                where += " AND r.ToStoreId IN @ToStoreId";
            }

            //未指定门店查询时，过滤掉与当前账户的门店无关的调拨单
            var canViewStores = _context.CurrentAccount.StoreArray;
            if (condition.FromStoreId.IsNullOrEmpty() && condition.ToStoreId.IsNullOrEmpty() && canViewStores.Length > 0)
            {
                param.FromStoreId = canViewStores;
                param.ToStoreId = canViewStores;
                where += " AND (r.FromStoreId IN @FromStoreId OR r.ToStoreId IN @ToStoreId)";
            }

            if (condition.BatchNo.NotNull())
            {
                param.BatchNo = condition.BatchNo;
                where += " AND r.BatchNo = @BatchNo";
            }
            if (condition.SAPCode.IsNotEmpty())
            {
                where += " AND r.SAPCode = @SAPCode";
                param.SAPCode = condition.SAPCode;
            }
            if (condition.CreateOnFrom.HasValue)
            {
                where += " AND r.CreatedOn >= @CreateOnFrom ";
                param.CreateOnFrom = condition.CreateOnFrom.Value;
            }

            if (condition.CreateOnTo.HasValue)
            {
                where += " AND r.CreatedOn < @CreateOnTo ";
                param.CreateOnTo = condition.CreateOnTo.Value.AddDays(1);
            }

            if (condition.CategoryCode.IsNotEmpty())
            {
                where += " AND c.Code LIKE @CategoryCode ";
                param.CategoryCode = string.Format("{0}%", condition.CategoryCode);
            }

            if (condition.BrandCode.IsNotEmpty())
            {
                where += " AND b.Code = @BrandCode ";
                param.BrandCode = condition.BrandCode;
            }

            if (condition.ProductCode.IsNotEmpty())
            {
                where += " AND i.ProductCode LIKE @ProductCode ";
                param.ProductCode = string.Format("%{0}%", condition.ProductCode);
            }

            if (condition.ProductName.IsNotEmpty())
            {
                where += " AND i.ProductName LIKE @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
            }
            if (condition.ProductIds.IsNotEmpty())
            {
                where += "  AND P.ID IN @productIds ";
                param.productIds = condition.ProductIds.Split(',').ToIntArray();
            }
            if (condition.SNCode.IsNotEmpty())
            {
                where += " AND i.SNCode like @SNCode ";
                param.SNCode = string.Format("%{0}%", condition.SNCode);
            }
            if (condition.BrandIds.IsNotEmpty())
            {
                where += "  AND b.ID IN @BrandIds ";
                param.BrandIds = condition.BrandIds.Split(',').ToIntArray();
            }

            if (condition.IsPushSap.HasValue)
            {
                param.IsPushSap = condition.IsPushSap.Value;
                where += " AND r.IsPushSap = @IsPushSap";
            }

            var fields = @" 
                r.Id ,
                r.Code ,
                r.SapCode ,
                r.Type ,
                r.FromStoreId ,
                fs.Code AS FromStoreCode ,
                r.FromStoreName ,
                r.ToStoreName ,
                r.ToStoreId ,
                ts.Code AS ToStoreCode ,
                r.BatchNo ,
                r.Status ,
                r.Remark ,
                r.CreatedOn ,
                r.CreatedBy ,
                r.CreatedByName ,
                r.UpdatedOn ,
                r.UpdatedBy ,
                r.UpdatedByName ,
                r.IsPushSap ,
                i.ProductCode ,
                p.Name AS ProductName ,
                p.Spec ,
                i.Quantity ,
                i.ActualShipmentQuantity ,
                i.ActualReceivedQuantity";
            var basicSql = @"
                SELECT  {0}
                FROM    TransferOrder r
                        INNER JOIN TransferOrderItem i on r.Id= i.TransferOrderId 
                        INNER JOIN Store fs ON r.FromStoreId = fs.Id
                        INNER JOIN Store ts ON r.ToStoreId = ts.Id
                        LEFT  JOIN Product p on p.Id = i.ProductId
		                LEFT  JOIN Category c on c.Id = p.CategoryId
		                LEFT  JOIN Brand b on b.Id = p.BrandId
                WHERE   1 = 1 " + where;
            var dataSql = basicSql.FormatWith(fields);

            if (page.toExcel)
            {
                return _db.DataBase.Query<TransferOrderDetailDto>(dataSql, param);
            }

            var countSql = basicSql.FormatWith("COUNT(1)");
            var pageSql = "{0} ORDER BY r.Id DESC OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY".FormatWith(dataSql, (page.PageIndex - 1) * page.PageSize, page.PageSize);

            page.Total = _db.DataBase.ExecuteScalar<int>(countSql, param);
            var data = _db.DataBase.Query<TransferOrderDetailDto>(pageSql, param);
            return data;
        }

        public TransferOrderDto GetTransferOrderById(int id)
        {
            var orderSql = @"
                SELECT  t.* 
                FROM    TransferOrder t
                WHERE   t.Id=@Id";
            var itemSql = @"
                SELECT  i.* ,
                        p.Code AS ProductCode ,
                        p.Name AS ProductName ,
                        p.Spec,
                        p.HasSNCode ,
                        si.Quantity - si.LockedQuantity AS InventoryQuantity
                FROM    TransferOrderItem i ,
                        Product p ,
                        StoreInventory si
                WHERE   i.ProductId = p.Id
                        AND i.ProductId = si.ProductId
                        AND si.StoreId = @StoreId
                        AND i.TransferOrderId = @TransferOrderId";
            var transferOrder = _db.DataBase.QuerySingle<TransferOrderDto>(orderSql, new { Id = id });
            transferOrder.Items = _db.DataBase.Query<TransferOrderItemDto>(itemSql, new { TransferOrderId = id, StoreId = transferOrder.FromStoreId }).ToList();
            return transferOrder;
        }

        public void Create(TransferCreateModel model)
        {
            var transferOrder = model.MapTo<TransferOrder>();
            var currentAccount = _context.CurrentAccount;
            transferOrder.CreatedOn = DateTime.Now;
            transferOrder.CreatedBy = currentAccount.AccountId;
            transferOrder.CreatedByName = currentAccount.NickName;

            var fromStore = _db.Table<Store>().FirstOrDefault(s => s.Id == transferOrder.FromStoreId);
            var toStore = _db.Table<Store>().FirstOrDefault(s => s.Id == transferOrder.ToStoreId);

            transferOrder.SetStore(fromStore, toStore);
            Ensure.When(() => !currentAccount.HaveAllStores).Then(() => Ensure.In(toStore.Id, currentAccount.StoreArray, "抱歉，您没有该门店的操作权限。"));

            transferOrder.Items.ForEach(item =>
            {
                var product = _db.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "商品不存在。");

                item.ProductCode = product.Code;
                item.Unit = product.Unit;

                Ensure.GreaterThan(item.Quantity, 0, "调拨数量必须大于0。");
                Ensure.False(product.HasSNCode && item.Quantity > SystemConfig.ItemMaxSNCodeQuantity, "商品{0}一次最多只能调拨{1}个串码。".FormatWith(product.Code, SystemConfig.ItemMaxSNCodeQuantity));

                var storeInventory = _db.Table<StoreInventory>().FirstOrDefault(iv => iv.StoreId == fromStore.Id && iv.ProductId == item.ProductId);
                Ensure.NotNull(storeInventory, "调出门店不存在商品【{0}】。".FormatWith(product.Code));
                Ensure.GreaterOrEqualThan(storeInventory.Quantity - storeInventory.LockedQuantity, item.Quantity, "调出门店商品【{0}】库存不足".FormatWith(product.Code));

                var productPurchasePrice = _purchasePriceService.QueryCurrentPurchasePrice(item.ProductId);
                //Ensure.NotNull(productPurchasePrice, "未维护商品【{0}】采购价。".FormatWith(product.Code));
                if (productPurchasePrice != null)
                    item.Price = productPurchasePrice.PurchasePrice;
            });

            transferOrder.Code = _sequenceService.GenerateNewCode(BillIdentity.TransferOrder);
            transferOrder.Status = TransferStatus.Initial;
            transferOrder.IsPushSap = false;
            transferOrder.SetType(fromStore);

            _db.Insert(transferOrder);
            _db.SaveChange();

            //记录单据处理历史
            _db.Insert(new ProcessHistory(transferOrder.CreatedBy, transferOrder.CreatedByName, transferOrder.Status.Value(), transferOrder.Id, BillIdentity.TransferOrder.Name(), "创建调拨单"));
            _db.SaveChange();

            //门店间调拨，直接推SAP
            if (transferOrder.Type == TransferType.StoreTransfer)
            {
                this.SubmitToSap(transferOrder);
            }
        }

        public void SubmitToSap(TransferOrder transferOrder)
        {
            transferOrder.Items = _db.Table<TransferOrderItem>().Where(m => m.TransferOrderId == transferOrder.Id).ToList();

            var order = new Order()
            {
                Code = transferOrder.Code,
                OrderType = Guoc.BigMall.Domain.Objects.OrderType.ZUBA,
                OrderDate = transferOrder.CreatedOn.Value,
                Remark = transferOrder.Remark,
                SupplierCode = "P010",
                Items = transferOrder.Items.Select(item =>
                {
                    var product = _db.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                    var category = _db.Table<Category>().FirstOrDefault(c => c.Id == product.CategoryId);
                    return new OrderItem()
                    {
                        ItemRow = item.Id,
                        ProductCode = item.ProductCode,
                        CategoryCode = category.Code,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        FromStoreCode = transferOrder.FromStoreCode,
                        ToStoreCode = transferOrder.ToStoreCode,
                    };
                }).ToList(),
            };
            _sapService.SubmitPurchaseOrder(order);//推送SAP

            //记录sap信息
            transferOrder.SapCode = order.SapCode;
            transferOrder.Items.ForEach(item =>
            {
                var itemResult = order.Items.First(m => m.ItemRow == item.Id);
                item.SapRow = itemResult.SapRow;
            });

            transferOrder.Status = TransferStatus.WaitShipping;
            transferOrder.IsPushSap = true;
            _db.Update(transferOrder);
            _db.Update(transferOrder.Items.ToArray());
            _db.SaveChange();
        }

        public void Edit(TransferEditModel model)
        {
            var transferOrder = _db.Table<TransferOrder>().FirstOrDefault(t => t.Id == model.Id);
            Ensure.NotNull(transferOrder, "调拨单不存在。");
            Ensure.In(transferOrder.Status, new[] { TransferStatus.Initial, TransferStatus.WaitAudit, TransferStatus.Reject }, "调拨单状态必须是“初始”、“待审”、“驳回”才允许编辑。");

            model.MapTo(transferOrder);
            transferOrder.IsPushSap = false;
            var currentAccount = _context.CurrentAccount;
            transferOrder.Edit(currentAccount.AccountId, currentAccount.NickName, DateTime.Now);

            var fromStore = _db.Table<Store>().FirstOrDefault(s => s.Id == transferOrder.FromStoreId);
            var toStore = _db.Table<Store>().FirstOrDefault(s => s.Id == transferOrder.ToStoreId);

            transferOrder.SetStore(fromStore, toStore);
            Ensure.When(() => !currentAccount.HaveAllStores).Then(() => Ensure.In(toStore.Id, currentAccount.StoreArray, "抱歉，您没有该门店的操作权限。"));

            transferOrder.Items.ForEach(item =>
            {
                var product = _db.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "商品不存在。");

                item.ProductCode = product.Code;
                item.Unit = product.Unit;

                Ensure.GreaterThan(item.Quantity, 0, "调拨数量必须大于0。");

                var storeInventory = _db.Table<StoreInventory>().FirstOrDefault(iv => iv.StoreId == fromStore.Id && iv.ProductId == item.ProductId);
                Ensure.NotNull(storeInventory, "调出门店不存在商品【{0}】。".FormatWith(product.Code));
                Ensure.GreaterOrEqualThan(storeInventory.Quantity - storeInventory.LockedQuantity, item.Quantity, "调出门店商品【{0}】库存不足".FormatWith(product.Code));

                var productPurchasePrice = _purchasePriceService.QueryCurrentPurchasePrice(item.ProductId);
                //Ensure.NotNull(productPurchasePrice, "未维护商品【{0}】采购价。".FormatWith(product.Code));
                if (productPurchasePrice != null)
                    item.Price = productPurchasePrice.PurchasePrice;

                item.TransferOrderId = transferOrder.Id;
            });

            transferOrder.SetType(fromStore);

            _db.Delete<TransferOrderItem>(t => t.TransferOrderId == transferOrder.Id);
            _db.Insert(transferOrder.Items.ToArray());
            _db.Update(transferOrder);
            _db.SaveChange();

            //门店间调拨，直接推SAP
            if (transferOrder.Type == TransferType.StoreTransfer)
            {
                this.SubmitToSap(transferOrder);
            }
        }

        public void ApplyAudit(int transferOrderId)
        {
            var transferOrder = _db.Table<TransferOrder>().FirstOrDefault(t => t.Id == transferOrderId);
            Ensure.NotNull(transferOrder, "调拨单不存在。");

            var currentAccount = _context.CurrentAccount;
            transferOrder.ApplyAudit(currentAccount.AccountId, currentAccount.NickName, DateTime.Now);

            //记录单据处理历史
            _db.Insert(new ProcessHistory(transferOrder.CreatedBy, transferOrder.CreatedByName, transferOrder.Status.Value(), transferOrder.Id, BillIdentity.TransferOrder.Name(), "申请审核"));

            _db.Update(transferOrder);
            _db.SaveChange();
        }

        public void PassAudit(int transferOrderId, string auditRemark)
        {
            var transferOrder = _db.Table<TransferOrder>().FirstOrDefault(t => t.Id == transferOrderId);
            Ensure.NotNull(transferOrder, "调拨单不存在。");

            var currentAccount = _context.CurrentAccount;
            transferOrder.PassAudit(currentAccount.AccountId, currentAccount.NickName, DateTime.Now, auditRemark);

            //记录单据处理历史
            _db.Insert(new ProcessHistory(transferOrder.CreatedBy, transferOrder.CreatedByName, transferOrder.Status.Value(), transferOrder.Id, BillIdentity.TransferOrder.Name(), "审核通过"));

            _db.Update(transferOrder);
            _db.SaveChange();

            //通知Sap生成调拨单
            this.SubmitToSap(transferOrder);

            //推送SAP成功后，调拨单状态变为待发货
            transferOrder.IsPushSap = true;
            transferOrder.Status = TransferStatus.WaitShipping;
            _db.Update(transferOrder);
            _db.SaveChange();
        }

        public void RejectAudit(int transferOrderId, string auditRemark)
        {
            var transferOrder = _db.Table<TransferOrder>().FirstOrDefault(t => t.Id == transferOrderId);
            Ensure.NotNull(transferOrder, "调拨单不存在。");

            var currentAccount = _context.CurrentAccount;
            transferOrder.RejectAudit(currentAccount.AccountId, currentAccount.NickName, DateTime.Now, auditRemark);

            //记录单据处理历史
            _db.Insert(new ProcessHistory(transferOrder.CreatedBy, transferOrder.CreatedByName, transferOrder.Status.Value(), transferOrder.Id, BillIdentity.TransferOrder.Name(), "审核驳回"));

            _db.Update(transferOrder);
            _db.SaveChange();
        }

        //public void Cancel(int transferOrderId)
        //{
        //    var transferOrder = _db.Table<TransferOrder>().FirstOrDefault(t => t.Id == transferOrderId);
        //    Ensure.NotNull(transferOrder, "调拨单不存在。");

        //    var currentAccount = _context.CurrentAccount;
        //    transferOrder.Cancel(currentAccount.AccountId, currentAccount.NickName, DateTime.Now);

        //    //记录单据处理历史
        //    _db.Insert(new ProcessHistory(transferOrder.CreatedBy, transferOrder.CreatedByName, transferOrder.Status.Value(), transferOrder.Id, BillIdentity.TransferOrder.Name(), "单据作废"));

        //    _db.Update(transferOrder);
        //    _db.SaveChange();
        //}

        public void OutStock(TransferStockOutModel model)
        {
            var transferOrder = _db.Table<TransferOrder>().FirstOrDefault(t => t.Id == model.Id);
            Ensure.NotNull(transferOrder, "调拨单不存在。");
            Ensure.False(model.Items.GroupBy(m => m.Id).Any(g => g.Count() > 1), "调拨单出库明细重复。");

            var currentAccount = _context.CurrentAccount;
            Ensure.When(() => !currentAccount.HaveAllStores).Then(() => Ensure.In(transferOrder.FromStoreId, currentAccount.StoreArray, "抱歉，您没有该门店的操作权限。"));

            transferOrder.OutStock(currentAccount.AccountId, currentAccount.NickName, DateTime.Now);

            var storeInventoryModel = new StockOutModel()
            {
                BillId = transferOrder.Id,
                BillCode = transferOrder.Code,
                BillType = BillIdentity.TransferOrder,
                StoreId = transferOrder.FromStoreId,
                CreatedBy = currentAccount.AccountId,
                CreatedOn = DateTime.Now,
            };

            model.Items.ForEach(m =>
            {
                if (m.ActualShipmentQuantity == 0) return;

                var item = _db.Table<TransferOrderItem>().FirstOrDefault(p => p.Id == m.Id && p.TransferOrderId == transferOrder.Id);
                Ensure.NotNull(item, "调拨明细不存在。");

                var product = _db.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "调拨商品不存在。");
                Ensure.Between(m.ActualShipmentQuantity, 1, item.Quantity, "商品{0}出库数量必须 > 0 且 ≤ 调拨数量。".FormatWith(product.Code));
                Ensure.EqualThan(m.SNCodes.Distinct().Count(), m.SNCodes.Count, "出库商品{0}包含重复的串码。".FormatWith(product.Code));

                item.ActualShipmentQuantity = m.ActualShipmentQuantity;
                item.SNCodes = m.SNCodes.Count > 0 ? string.Join(",", m.SNCodes) : null;
                _db.Update(item);

                var storeInventoryItemModel = new StockOutItemModel()
                {
                    ProductId = item.ProductId,
                    Quantity = m.ActualShipmentQuantity,
                    SNCodes = m.SNCodes.Select(snCode => new SNCodeModel(snCode)).ToList()
                };
                storeInventoryModel.Items.Add(storeInventoryItemModel);

                transferOrder.Items.Add(item);
            });

            var result = _storeInventoryFacade.OutStock(_db, storeInventoryModel);

            //记录单据处理历史
            _db.Insert(new ProcessHistory(currentAccount.AccountId, currentAccount.NickName, transferOrder.Status.Value(), transferOrder.Id, BillIdentity.TransferOrder.Name(), "调拨出库"));

            _db.Update(transferOrder);
            _db.SaveChange();

            //推送SAP，SAP生成对应发货单
            this.SapOutStock(transferOrder, result.HistoryCode, result.CreatedOn, currentAccount);
        }

        public void SapOutStock(TransferOrder transferOrder, string historyCode, DateTime stockOutDate, AccountInfo currentAccount)
        {
            var stockOutHistories = _db.Table<StoreInventoryHistorySAP>().Where(h => h.Code == historyCode).ToList();
            var transferStockOut = new TransferStockOut()
            {
                TransferCode = transferOrder.Code,
                SapTransferCode = transferOrder.SapCode,
                StockOutCode = historyCode,
                StockOutDate = stockOutDate,
                SupplierCode = "P010",
                Items = stockOutHistories.Select(history =>
                {
                    var item = transferOrder.Items.First(h => h.ProductId == history.ProductId);
                    return new TransferStockOutItem()
                    {
                        StockOutRow = history.Id,
                        SapTransferRow = item.SapRow,
                        ProductCode = item.ProductCode,
                        Quantity = history.Quantity,
                        SNCodes = history.SNCodes,
                        Unit = item.Unit,
                        FromStoreCode = history.StoreCode,
                    };
                }).ToList(),
            };
            _sapService.TransferOrderOutStock(transferStockOut);//推送SAP

            //记录SAP出库单信息
            transferStockOut.Items.ForEach(item =>
            {
                //_db.Update<StoreInventoryHistorySAP>(h =>
                //    new StoreInventoryHistorySAP()
                //    {
                //        SAPCode = transferStockOut.SapStockOutCode,
                //        SAPRow = item.SapStockOutRow
                //    },
                //    h => h.Code == result.HistoryCode && h.Id == item.StockOutRow);

                var history = _db.Table<StoreInventoryHistorySAP>().FirstOrDefault(h => h.Code == historyCode && h.Id == item.StockOutRow);
                history.SAPCode = transferStockOut.SapStockOutCode;
                history.SAPRow = item.SapStockOutRow;
                _db.Update(history);
            });

            //推送SAP成功后，调拨单状态变为待收货
            transferOrder.IsPushSap = true;
            transferOrder.Status = TransferStatus.WaitReceiving;

            //记录单据处理历史
            _db.Insert(new ProcessHistory(currentAccount.AccountId, currentAccount.NickName, transferOrder.Status.Value(), transferOrder.Id, BillIdentity.TransferOrder.Name(), "SAP出库成功"));

            _db.Update(transferOrder);
            _db.SaveChange();
        }

        public void InStock(TransferStockInModel model)
        {
            var transferOrder = _db.Table<TransferOrder>().FirstOrDefault(t => t.Id == model.Id);
            Ensure.NotNull(transferOrder, "调拨单不存在。");
            Ensure.False(model.Items.GroupBy(m => m.Id).Any(g => g.Count() > 1), "调拨单出库明细重复。");

            var currentAccount = _context.CurrentAccount;
            Ensure.When(() => !currentAccount.HaveAllStores).Then(() => Ensure.In(transferOrder.ToStoreId, currentAccount.StoreArray, "抱歉，您没有该门店的操作权限。"));

            transferOrder.InStock(currentAccount.AccountId, currentAccount.NickName, DateTime.Now);

            var stockInModel = new StockInModel()
            {
                InStockType = InStockType.Normal,
                StockInBillId = transferOrder.Id,
                StockInBillCode = transferOrder.Code,
                StockInBillType = BillIdentity.TransferOrder,
                StoreId = transferOrder.ToStoreId,
                CreatedBy = currentAccount.AccountId,
                CreatedOn = DateTime.Now,
            };

            model.Items.ForEach(m =>
            {
                var item = _db.Table<TransferOrderItem>().FirstOrDefault(p => p.Id == m.Id && p.TransferOrderId == transferOrder.Id);
                Ensure.NotNull(item, "调拨明细不存在。");

                var product = _db.Table<Product>().FirstOrDefault(p => p.Id == item.ProductId);
                Ensure.NotNull(product, "调拨商品不存在。");

                Ensure.EqualThan(m.ActualReceivedQuantity, item.ActualShipmentQuantity, "商品{0}收货数量与发货数量不一致。".FormatWith(product.Code));

                if (item.ActualShipmentQuantity == 0) return;

                Ensure.EqualThan(m.StockInSNCodes.Distinct().Count(), m.StockInSNCodes.Count, "出库商品{0}包含重复的串码。".FormatWith(product.Code));

                item.ActualReceivedQuantity = m.ActualReceivedQuantity;
                _db.Update(item);

                var stockOutInventoryBatchList = _storeInventoryFacade.GetStockOutInventoryBatch(transferOrder.FromStoreId, transferOrder.Id, BillIdentity.TransferOrder, item.ProductId);
                Ensure.EqualThan(stockOutInventoryBatchList.Sum(b => Math.Abs(b.ChangeQuantity)), m.ActualReceivedQuantity, "商品{0}实收数量与出库数量不一致。".FormatWith(product.Code));

                if (m.StockInSNCodes.Count > 0)
                {
                    var stockOutSNCodes = stockOutInventoryBatchList.Select(b => b.SNCode).ToList();
                    m.StockInSNCodes.ForEach(snCode =>
                    {
                        Ensure.In(snCode, stockOutSNCodes, "商品{0}串码{1}不包含在调拨单的出库列表中。".FormatWith(product.Code, snCode));
                    });
                    Ensure.EqualThan(m.StockInSNCodes.Count, stockOutSNCodes.Count, "商品{0}入库串码和调出串码数量不一致。".FormatWith(product.Code));
                }

                stockOutInventoryBatchList.ForEach(b =>
                {
                    var storeInventoryItemModel = new StockInItemModel()
                    {
                        ProductId = item.ProductId,
                        SupplierId = b.SupplierId,
                        CostPrice = b.Price,
                        ContractPrice = b.ContractPrice,
                        ProductionDate = b.ProductionDate,
                        ShelfLife = b.ShelfLife,
                        Quantity = Math.Abs(b.ChangeQuantity),
                        SNCode = b.SNCode
                    };
                    stockInModel.Items.Add(storeInventoryItemModel);
                });
            });

            var result = _storeInventoryFacade.InStock(_db, stockInModel);

            //记录单据处理历史
            _db.Insert(new ProcessHistory(currentAccount.AccountId, currentAccount.NickName, transferOrder.Status.Value(), transferOrder.Id, BillIdentity.TransferOrder.Name(), "调拨入库"));

            _db.Update(transferOrder);
            _db.SaveChange();

            //推送SAP，SAP生成对应发货单
            this.SapInStock(transferOrder, result.HistoryCode, result.CreatedOn, currentAccount);
        }

        public void SapInStock(TransferOrder transferOrder, string historyCode, DateTime stockOutDate, AccountInfo currentAccount)
        {
            transferOrder.Items = _db.Table<TransferOrderItem>().Where(n => n.TransferOrderId == transferOrder.Id).ToList();
            var stockOutHistories = _db.Table<StoreInventoryHistorySAP>().Where(h => h.StoreId == transferOrder.FromStoreId && h.BillCode == transferOrder.Code && h.BillType == BillIdentity.TransferOrder && h.Type == StoreInventoryHistorySapType.OutStock).ToList();
            var stockInHistories = _db.Table<StoreInventoryHistorySAP>().Where(h => h.Code == historyCode).ToList();
            var transferStockOut = new TransferStockIn()
            {
                TransferCode = transferOrder.Code,
                SapStockOutCode = stockOutHistories.First().SAPCode,
                StockInCode = historyCode,
                StockInDate = stockOutDate,
                Items = stockInHistories.Select(stockInHistory =>
                {
                    var item = transferOrder.Items.First(m => m.ProductId == stockInHistory.ProductId);
                    var stockOutHistory = stockOutHistories.First(h => h.ProductId == stockInHistory.ProductId);
                    return new TransferStockInItem()
                    {
                        SapStockOutRow = stockOutHistory.SAPRow,
                        StockInRow = stockInHistory.Id,
                        ProductCode = stockInHistory.ProductCode,
                        Quantity = stockInHistory.Quantity,
                        SNCodes = stockInHistory.SNCodes,
                        Unit = item.Unit,
                        ToStoreCode = stockInHistory.StoreCode,
                    };
                }).ToList(),
            };
            _sapService.TransferOrderInStock(transferStockOut);//推送SAP

            //记录SAP入库单信息
            transferStockOut.Items.ForEach(item =>
            {
                //_db.Update<StoreInventoryHistorySAP>(h =>
                //    new StoreInventoryHistorySAP()
                //    {
                //        SAPCode = transferStockOut.SapStockInCode,
                //        SAPRow = item.SapStockInRow
                //    },
                //    h => h.Code == result.HistoryCode && h.Id == item.StockInRow);

                var history = _db.Table<StoreInventoryHistorySAP>().FirstOrDefault(h => h.Code == historyCode && h.Id == item.StockInRow);
                history.SAPCode = transferStockOut.SapStockInCode;
                history.SAPRow = item.SapStockInRow;
                _db.Update(history);
            });

            //推送SAP成功后，调拨单状态变为已完成
            transferOrder.IsPushSap = true;
            transferOrder.Status = TransferStatus.Finished;

            //记录单据处理历史
            _db.Insert(new ProcessHistory(currentAccount.AccountId, currentAccount.NickName, transferOrder.Status.Value(), transferOrder.Id, BillIdentity.TransferOrder.Name(), "SAP入库成功"));

            _db.Update(transferOrder);
            _db.SaveChange();
        }


        public List<TransferOrderPrintDto> GetPrintList(string ids)
        {
            if (string.IsNullOrEmpty(ids)) { throw new FriendlyException("单号不能为空"); }
            var sql = @"select o.*,p.Name as ProductName,p.Code  as ProductCode,i.Quantity,i.ActualShipmentQuantity,i.ActualReceivedQuantity from TransferOrder o inner join TransferOrderItem i on o.Id = i.TransferOrderId
left join Product p on p.Id = i.ProductId
where o.Id in @Id ";
            var idArray = ids.Split(',').ToIntArray();
            var data = _db.DataBase.Query<TransferOrderPrintDto>(sql, new { id = idArray }).ToList();
            return data;
        }
    }
}
