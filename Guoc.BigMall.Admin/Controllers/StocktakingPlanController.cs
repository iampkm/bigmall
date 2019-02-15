
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
using Guoc.BigMall.Infrastructure;
namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class StocktakingPlanController : Controller
    {
        IDBContext _db;
        IContextFacade _context;
        IAuthenticationService _authenticationService;
        IStocktakingPlannFacade _inventoryPlanFacade;
        IExcel _execl;

        public StocktakingPlanController(IDBContext dbcontext, IAuthenticationService authenticationService, IContextFacade context, IStocktakingPlannFacade inventoryPlanFacade, IExcel execl)
        {
            this._db = dbcontext;
            this._context = context;
            this._authenticationService = authenticationService;
            this._inventoryPlanFacade = inventoryPlanFacade;
            this._execl = execl;
        }

        // GET: InventoryPlan
        public ActionResult Index()
        {
            // ViewBag.Status = typeof(StocktakingPlanStatus).GetValueToDescription();
            ViewBag.Status = JsonConvert.SerializeObject(typeof(StocktakingPlanStatus).GetValueToDescription().ToArray());

            ViewBag.StoreId = _context.CurrentAccount.HaveAllStores ? "" : _context.CurrentAccount.StoreArray.FirstOrDefault().ToString();
            return View();
        }
        public ActionResult LoadDetailData(Pager page, SearchInventoryPlan condition)
        {
            if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            var rows = _inventoryPlanFacade.GetPageList(page, condition);

            return Json(new { success = true, data = rows, total = page.Total, sum = page.SumColumns });
        }

        public ActionResult ExportExecl(string selectStoreId)
        {
            if (string.IsNullOrEmpty(selectStoreId))
            {
                return Json(new { success = false, error = "请选择一个门店，再导出盘点表！" });
            }

            //本月开始时间
            //var startDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM") + "-01");
            //var endDate = DateTime.Parse(DateTime.Now.AddMonths(1).ToString("yyyy-MM") + "-01");
            int storeId = int.Parse(selectStoreId);
            var planList = _db.Table<StocktakingPlan>().FirstOrDefault(s => s.StoreId == storeId && s.Status >= StocktakingPlanStatus.ToBeInventory && s.Status < StocktakingPlanStatus.Complete);

            if (planList == null)
            {
                planList = new StocktakingPlan();
                //新增计划并导出
                planList.Id = _inventoryPlanFacade.CreatePlan(storeId, _context.CurrentAccount.AccountId, _context.CurrentAccount.UserName);
            }
            var rows = _inventoryPlanFacade.GetInventoryPlanProduct(planList.Id);
            var data = _execl.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
            var fileName = string.Format("盘点单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
            return File(data, "application/ms-excel", fileName);
        }
        [HttpPost]
        public JsonResult ImpExecl(List<PlanProductDto> planProductItems)
        {
            //try
            //{
            //string storeIds = _context.CurrentAccount.CanViewStores;

            //if (!string.IsNullOrEmpty(storeIds))
            //{
            //var arrStore = storeIds.Split(',');

            //if (arrStore.Count() != 1)
            //{
            //    return Json(new { success = false, error = "该用户无权限导入！" });
            //}
            //else
            //{
            //本月开始时间
            //var startDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM") + "-01");
            //var endDate = DateTime.Parse(DateTime.Now.AddMonths(1).ToString("yyyy-MM") + "-01");
            //int storeId = int.Parse(arrStore[0]);
            //s.CreatedOn >= startDate && s.CreatedOn < endDate &&
            //var planList = _db.Table<StocktakingPlan>().FirstOrDefault(s => s.StoreId == storeId && s.Status > 0 && s.Status < 4);

            //if (planList == null)
            //{
            //    return Json(new { success = false, error = "请先导出本月盘点表！" });
            //}

            _inventoryPlanFacade.InsertPlanItem(planProductItems);
            return Json(new { success = true });
            //}
            //}
            //else
            //{
            //    return Json(new { success = false, error = "该用户无权限导入！" });
            //}
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { success = false, error = ex.Message });
            //}
        }
        [HttpPost]
        public JsonResult Confirm(string storeId)
        {
            _inventoryPlanFacade.Confirm(int.Parse(storeId));
            return Json(new { success = true });
        }
    }
}