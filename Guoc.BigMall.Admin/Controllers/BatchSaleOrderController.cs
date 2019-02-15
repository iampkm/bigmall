using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.File;
using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using Newtonsoft.Json;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Infrastructure;

namespace Guoc.BigMall.Admin.Controllers
{
    public class BatchSaleOrderController : Controller
    {
        IExcel _excelService;
        IDBContext _db;

        IBatchSaleOrderFacade _batchSaleOrderFacede;
        IReturnBatchSaleOrderFacade _returnOrderFacade;
        IContextFacade _context;
        IProductFacade _productFacade;
        public BatchSaleOrderController(IDBContext db, IExcel iexcel, IContextFacade context, IReturnBatchSaleOrderFacade returnOrderFacade, IBatchSaleOrderFacade batchSaleOrderFacede, IProductFacade productFacade)
        {
            this._excelService = iexcel;
            this._db = db;
            this._context = context;
            this._returnOrderFacade = returnOrderFacade;
            this._batchSaleOrderFacede = batchSaleOrderFacede;
            this._productFacade = productFacade;
        }

        #region 出库
        public ActionResult StockOutIndex()
        {
            ViewBag.ShowStatus = (int)SaleOrderStatus.WaitOutStock;
            return View();
        }


        public ActionResult StockOut(string code)
        {
            var condition = new SearchSaleOrder() { OrderCode = code };
            var order = _batchSaleOrderFacede.GetBatchSaleOrderModel(condition);
            order.SetDefaultActualQuantity();
            ViewBag.BatchSaleOrder = JsonConvert.SerializeObject(order);
            return View();
        }

        [HttpPost]
        public ActionResult StockOut(SaleOrderOutStockModel model)
        {
            _batchSaleOrderFacede.OutStock(model);
            return Json(new { success = true });
        }

        public JsonResult AbandonSaleOrder(SaleOrderAbandonModel model)
        {
            _batchSaleOrderFacede.AbandonBatchSaleOrder(model);
            return Json(new { success = true });
        }
        public ActionResult GetBatchSaleOrderList(Pager page, SearchSaleOrder condition)
        {
            //if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            if (condition.StoreId.IsNullOrEmpty())
                condition.StoreId = _context.CurrentAccount.CanViewStores;

            var rows = _batchSaleOrderFacede.GetBatchSaleOrderModels(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("预售订单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }
        #endregion

        #region 入库
        public ActionResult StockInIndex()
        {
            ViewBag.ShowStatus = (int)ReturnSaleOrderStatus.WaitInStock;
            return View();
        }

        public ActionResult StockIn(string code)
        {
            var order = _returnOrderFacade.GetReturnBatchSaleOrderByCode(code);
            order.SetDefaultActualQuantity();
            ViewBag.BatchSaleOrder = JsonConvert.SerializeObject(order);
            return View();
        }

        [HttpPost]
        public ActionResult StockIn(ReturnBatchOrderModel model)
        {
            _returnOrderFacade.InStock(model);
            return Json(new { success = true });
        }

        public ActionResult GetReturnOrderList(Pager page, SearchSaleOrder condition)
        {
            //if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            if (condition.StoreId.IsNullOrEmpty())
                condition.StoreId = _context.CurrentAccount.CanViewStores;

            var rows = _returnOrderFacade.GetReturnBatchSaleOrders(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("销售订单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }

        #endregion

        #region  综合查询
        /// <summary>
        /// 综合查询
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchIndex()
        {
            ViewBag.OrderType = JsonConvert.SerializeObject(typeof(OrderType).GetValueToDescription().ToArray());
            ViewBag.OrderStatus = JsonConvert.SerializeObject(typeof(SaleOrderStatus).GetValueToDescription().ToArray());
            ViewBag.OrderRoStatus = JsonConvert.SerializeObject(typeof(ReturnSaleOrderStatus).GetValueToDescription().ToArray());
            return View();
        }

        public ActionResult GetCompositeSaleOrders(Pager page, SearchSaleOrder condition)
        {
            if (condition.StoreId.IsNullOrEmpty())
                condition.StoreId = _context.CurrentAccount.CanViewStores;
            var rows = _batchSaleOrderFacede.GetBatchSaleOrderListDetail(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("批发单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult ShowDetail(string code)
        {
            var condition = new SearchSaleOrder() { OrderCode = code };
            var order = _batchSaleOrderFacede.GetBatchSaleOrderModel(condition);
            ViewBag.SaleOrder = JsonConvert.SerializeObject(order);
            ViewBag.OutStock = (int)SaleOrderStatus.OutStock;
            ViewBag.RejectAudit = (int)SaleOrderStatus.Create;
            return View();
        }

        #endregion
        /// <summary>
        ///  扫串码出库
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="snCode"></param>
        /// <returns></returns>
        public JsonResult ScanSNCodeForStockOut(int storeId, string snCode)
        {
            var model = _productFacade.QueryProduct(storeId, snCode);
            if (model.Quantity != 1)
            {
                throw new FriendlyException("商品:{0} 无库存".FormatWith(snCode));
            }
            return Json(new { success = true, data = model, });
        }

        /// <summary>
        ///  扫串码入库
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="snCode"></param>
        /// <returns></returns>
        public JsonResult ScanSNCodeForStockIn(int storeId, string snCode)
        {
            var model = _productFacade.QueryProduct(storeId, snCode);
            if (model.Quantity != 0)
            {
                throw new FriendlyException("商品:{0} 已入库".FormatWith(snCode));
            }
            return Json(new { success = true, data = model, });
        }

        public JsonResult PostOrderToSap(string code)
        {
            _batchSaleOrderFacede.PostOrderToSap(code);
            return Json(new { success = true });
        }

        /// <summary>
        /// 打印出库单
        /// </summary>
        /// <param name="ids">多个ID 逗号分隔</param>
        /// <returns></returns>
        public ActionResult Print(string ids)
        {
            var model = _batchSaleOrderFacede.GetPrintList(ids);

            return PartialView("BatchSaleOrderPrintTemplate", model);
        }
    }
}