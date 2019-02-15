using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper.DBContext;
using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure.Extension;
using Newtonsoft.Json;
using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Infrastructure.File;


namespace Guoc.BigMall.Admin.Controllers
{
    public class ReportController : Controller
    {
         IDBContext _db;
        IContextFacade _context;
 
        IReportFacade _reportFacade;
        IExcel _execl;

        public ReportController(IDBContext dbcontext,  IContextFacade context, IReportFacade reportFacade, IExcel execl)
        {
            this._db = dbcontext;
            this._context = context;
       
            this._reportFacade = reportFacade;
            this._execl = execl;
        }
        // GET: Report
        public ActionResult PurchaseSaleInventorySummary()
        {
            return View();
        }

        public ActionResult LoadPurchaseSaleInventorySummaryData(Pager page, SearchPurchaseSaleInventorySummary condition)
        {
            if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            var rows = _reportFacade.GetPageList(page, condition);

            return Json(new { success = true, data = rows, total = page.Total, sum = page.SumColumns });
        }

        public ActionResult PurchaseSaleInventorySummaryExecl(Pager page, SearchPurchaseSaleInventorySummary condition)
        {
            page.IsPaging = false;
            var rows = _reportFacade.GetPageList(page, condition);
            var data = _execl.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
            var fileName = string.Format("进销存报表_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
            return File(data, "application/ms-excel", fileName);
        }

        public ActionResult SaleOrderItemSummary()
        {
            var accountList = _db.DataBase.Query<AccountDto>("SELECT Id,UserName FROM dbo.Account WHERE RoleId>1", null).ToList();
            ViewBag.CreateUserList = accountList;
            return View();
        }

        public ActionResult LoadSaleOrderItemSummaryData(Pager page, SearchSaleOrderItemSummary condition)
        {
            if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            page.IsPaging= condition.ToExcel ? false : true;
            var rows = _reportFacade.GetSaleOrderItemList(page, condition);
            if (condition.ToExcel)
            {
                var data = _execl.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("销售明细_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total, sum = page.SumColumns });
        }

        public ActionResult SaleOrderSummary()
        {
            var accountList = _db.DataBase.Query<AccountDto>("SELECT Id,UserName FROM dbo.Account WHERE RoleId>1", null).ToList();
            ViewBag.CreateUserList = accountList;
            return View();
        }

        public ActionResult LoadSaleOrderSummaryData(Pager page, SearchSaleOrderSummary condition)
        {
            if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            page.IsPaging = condition.ToExcel ? false : true;
            var rows = _reportFacade.GetSaleOrderSummaryList(page, condition);
            if (condition.ToExcel)
            {
                var data = _execl.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("销售汇总_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }
            return Json(new { success = true, data = rows, total = page.Total, sum = page.SumColumns });
        }
        public ActionResult StoreInventory()
        {
            return View();
        }
        public ActionResult Profit()
        {
            var dir = EnumExtension.GetValueToDescription(typeof(BillIdentity));
            ViewBag.BillType = dir.Where(n => n.Key == (int)BillIdentity.SaleOrder || n.Key == (int)BillIdentity.SaleRefund || n.Key == (int)BillIdentity.BatchOrder
                || n.Key == (int)BillIdentity.BatchRefund || n.Key == (int)BillIdentity.PreSaleOrder || n.Key == (int)BillIdentity.PreSaleRefund || n.Key == (int)BillIdentity.ExchangeOrder);
             
          
            return View();
        }

        public ActionResult LoadProfitData(Pager page, SearchProfit condition)
        {
            if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            page.IsPaging = condition.ToExcel ? false : true;
            var rows = _reportFacade.GetOrderProfitList(page, condition);
            if (condition.ToExcel)
            {
                var data = _execl.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("利润报表_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }
            return Json(new { success = true, data = rows, total = page.Total, sum = page.SumColumns });
        }

    }
}