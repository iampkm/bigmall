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
using System.Configuration;
namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class PurchaseController : Controller
    {
        IDBContext _db;
        IExcel _excelService;
        IContextFacade _context;
        IAuthenticationService _authenticationService;
        IPurchaseFacade _purchaseFacede;

        public PurchaseController(IDBContext dbcontext, IAuthenticationService authenticationService, IContextFacade context, IPurchaseFacade purchaseFacede, IExcel excel)
        {
            this._db = dbcontext;
            this._context = context;
            this._excelService = excel;
            this._authenticationService = authenticationService;
            this._purchaseFacede = purchaseFacede;

        }

        public ActionResult Index()
        {

            ViewBag.Status = typeof(CBPurchaseOrderStatus).GetValueToDescription();
            ViewBag.BillTypes = typeof(PurchaseOrderBillType).GetValueToDescription();
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseOrder;

            return View();
        }
        public ActionResult LoadDetailData(Pager page, SearchPurchaseOrder condition)
        {
            if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            var rows = _purchaseFacede.GetDetailList(page, condition);
            return Json(new { success = true, data = rows, total = page.Total, sum = page.SumColumns });
        }
        public JsonResult GetDetailList(string id)
        {
            var rows = _purchaseFacede.GetOrderList(id);
            var amount = rows.Sum(n => n.TotalAmount);
            return Json(new { success = true, data = rows, totalAmount = amount });
        }
        public ActionResult View(int id)
        {
            var model = _purchaseFacede.GetById(id);
            ViewBag.PurchaseOrderItems = JsonConvert.SerializeObject(model.Items.ToArray());
            return View(model);
        }
        public ActionResult WaitedReceive()
        {
            ViewBag.Status = (int)CBPurchaseOrderStatus.Audited;
            ViewBag.BillTypes = typeof(PurchaseOrderBillType).GetValueToDescription();
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseOrder;
            return View();
        }
        public ActionResult Receive(int id)
        {
            this.ReceiveLimite(id);
            var model = _purchaseFacede.GetReceiveById(id);

            ////设置默认实收 = 实发
            //model.Items.ForEach((item) =>
            //{
            //    item.ActualQuantity = item.ActualQuantity == 0 ? item.ActualShipQuantity : item.ActualQuantity;
            //});
            ViewBag.PurchaseOrderItems = JsonConvert.SerializeObject(model.Items.ToArray());
            ViewBag.ShowClose = DateTime.Now.CompareTo(model.CreatedOn.AddDays(7)) > 0 ? "true" : "false";
            return View(model);
        }
        public void ReceiveLimite(int purchaseOrderId)
        {
            var model = _db.Table<PurchaseOrder>().FirstOrDefault(x => x.Id == purchaseOrderId);
            var set = ConfigurationManager.AppSettings["PurchaseOrderCloseSet"];
            if (string.IsNullOrWhiteSpace(set))
                throw new FriendlyException("Config未增加【PurchaseOrderCloseSet】配置");
            var setupEntity = _db.Table<SystemSetup>().FirstOrDefault(n => n.Name == set);
            if (setupEntity == null)
                throw new FriendlyException("系统设置表中未添加自动关单时间配置");
            int days;
            if (!int.TryParse(setupEntity.Value, out days))
            {
                throw new FriendlyException("自动关单时间必须设置成数字");
            }
            TimeSpan span = DateTime.Now - model.CreatedOn;
            if (span.Days > days)
            {
                throw new FriendlyException("超过收货时间限制，无法收货！");
            }
        }
        public ActionResult Print(int id)
        {
            var model = _purchaseFacede.GetById(id);
            var store = _db.Table<Store>().FirstOrDefault(n => n.Id == model.StoreId);//.QuerySingle<Store>(n=>n.  model.StoreId);
            var area = _db.Table<Area>().FirstOrDefault(n => n.Id == store.AreaId);
            model.Address = area.FullName + " " + store.Address;
            return PartialView("PurchaseOrderTemplate", model);
        }
        [HttpPost]
        public JsonResult Close(int id)
        {
            _purchaseFacede.Close(id);
            return Json(new { success = true });
        }
        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult StockIn(PurchaseOrderModel model)
        {
            this.ReceiveLimite(model.Id);
            model.EditBy = _context.CurrentAccount.AccountId;
            model.EditByName = _context.CurrentAccount.NickName;
            _purchaseFacede.StockIn(model);
            return Json(new { success = true });
        }
        private void SetUserAuthention()
        {
            ViewBag.View = _context.CurrentAccount.ShowSelectStore() ? "true" : "false";
            //ViewBag.StoreId = _context.CurrentAccount.StoreId;
            ViewBag.StoreName = _context.CurrentAccount.StoreName;
            ViewBag.StoreCode = _context.CurrentAccount.StoreCode;
        }
        #region 退货
        /// <summary>
        /// 退货综合查询
        /// </summary>
        /// <returns></returns>
        public ActionResult RefundIndex()
        {
            ViewBag.Creater = _db.DataBase.Query<Account>("select * from account", null).ToDictionary(k => k.Id, v => v.NickName);
            ViewBag.Status = typeof(CBPurchaseOrderStatus).GetValueToDescription();
            ViewBag.BillTypes = typeof(PurchaseOrderBillType).GetValueToDescription();
            ViewBag.OrderType = typeof(PurchaseOrderType).GetValueToDescription();
            return View();
        }
        public ActionResult LoadRefundData(Pager page, SearchPurchaseOrder condition)
        {
            if (string.IsNullOrEmpty(condition.StoreId) || condition.StoreId == "0") { condition.StoreId = _context.CurrentAccount.CanViewStores; }
            var rows = _purchaseFacede.GetRefundDetailList(page, condition);

            if (page.toExcel)
            {
                var data = _excelService.WriteToExcelStream(rows.ToList(), ExcelVersion.Above2007, false, true).ToArray();
                var fileName = string.Format("采购单_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));
                return File(data, "application/ms-excel", fileName);
            }

            return Json(new { success = true, data = rows, total = page.Total, sum = page.SumColumns });
        }

        public ActionResult RefundView(int Id)
        {
            var model = _purchaseFacede.GetById(Id);
            //ViewBag.CanEdit = model.BillType == PurchaseOrderBillType.StockOrder ? "false" : "true";
            ViewBag.PurchaseOrderItems = JsonConvert.SerializeObject(model.Items.ToArray());
            //查询处理流程：
            //var logs = _query.FindAll<ProcessHistory>(n => n.FormId == id && n.FormType == BillIdentity.PurchaseRefundOrder.ToString());
            //ViewBag.Logs = logs;

            return View(model);
        }
        /// <summary>
        /// 作废采购订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public JsonResult Cancel(int id, string reason)
        {
            _purchaseFacede.Cancel(id, _context.CurrentAccount.AccountId, _context.CurrentAccount.NickName, reason);
            return Json(new { success = true });
        }
        /// <summary>
        /// 退货单制作
        /// </summary>
        /// <returns></returns>
        public ActionResult RefundCreateIndex()
        {
            ViewBag.Creater = _db.DataBase.Query<Account>("select * from account", null).ToDictionary(k => k.Id, v => v.NickName);
            ViewBag.Status = ViewBag.Status = ((int)CBPurchaseOrderStatus.Create).ToString() + "," + ((int)CBPurchaseOrderStatus.Reject).ToString();
            ViewBag.BillType = (int)PurchaseOrderBillType.StoreOrder;
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseReturn;
            return View();
        }

        public ActionResult RefundCreate()
        {
            ViewBag.Status = CBPurchaseOrderStatus.Create.Description();
            ViewBag.CreatedByName = _context.CurrentAccount.NickName;
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseReturn;
            SetUserAuthention();
            return View();
        }
        public JsonResult GetRefundProduct(Pager page, SearchProduct condition)
        {

            var rows = _purchaseFacede.GetRefundProduct(page, condition);

            return Json(new { success = true, data = rows, total = page.Total });
        }





        [HttpPost]
        public JsonResult Create(PurchaseOrderModel model)
        {
            model.EditBy = _context.CurrentAccount.AccountId;
            model.EditByName = _context.CurrentAccount.NickName;
            _purchaseFacede.RefundCreate(model);
            return Json(new { success = true, ids = model.Ids });
        }

        public ActionResult RefundEdit(int Id)
        {
            var model = _purchaseFacede.GetById(Id);
            if (model.BillType == PurchaseOrderBillType.StockOrder)
                throw new FriendlyException("此单据为仓库退货单，无法编辑！");
            ViewBag.PurchaseOrderItems = JsonConvert.SerializeObject(model.Items.ToArray());
            //查询处理流程：
            //var logs = _query.FindAll<ProcessHistory>(n => n.FormId == id && n.FormType == BillIdentity.PurchaseRefundOrder.ToString());
            //ViewBag.Logs = logs;

            return View(model);
        }
        [HttpPost]
        public JsonResult RedundEdit(PurchaseOrderModel model)
        {
            model.EditBy = _context.CurrentAccount.AccountId;
            model.EditByName = _context.CurrentAccount.NickName;
            _purchaseFacede.RefundEdit(model);
            return Json(new { success = true, ids = model.Ids });
        }


        public ActionResult RefundSendIndex()
        {
            ViewBag.Creater = _db.DataBase.Query<Account>("select * from account", null).ToDictionary(k => k.Id, v => v.NickName);
            ViewBag.Status = (int)CBPurchaseOrderStatus.Audited;
            ViewBag.BillType = typeof(PurchaseOrderBillType).GetValueToDescription();
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseReturn;
            return View();
        }
        [HttpPost]
        public ActionResult OutStock(PurchaseOrderModel model)
        {
            model.EditBy = _context.CurrentAccount.AccountId;
            model.EditByName = _context.CurrentAccount.NickName;
            _purchaseFacede.StockOut(model);
            return Json(new { success = true });

        }

        public ActionResult RefundSend(int id)
        {

            var model = _purchaseFacede.GetById(id);

            ViewBag.PurchaseOrderItems = JsonConvert.SerializeObject(model.Items.ToArray());
            return View(model);
        }


        #endregion

        #region 换货

        public ActionResult ChangeIndex()
        {
            ViewBag.Creater = _db.DataBase.Query<Account>("select * from account", null).ToDictionary(k => k.Id, v => v.NickName);
            ViewBag.Status = typeof(CBPurchaseOrderStatus).GetValueToDescription();
            ViewBag.BillType = (int)PurchaseOrderBillType.StoreOrder;
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseChange;
            return View();
        }


        public ActionResult ChangeView(int Id)
        {
            var model = _purchaseFacede.GetById(Id);
            ViewBag.PurchaseOrderItems = JsonConvert.SerializeObject(model.Items.ToArray());
            //查询处理流程：
            //var logs = _query.FindAll<ProcessHistory>(n => n.FormId == id && n.FormType == BillIdentity.PurchaseRefundOrder.ToString());
            //ViewBag.Logs = logs;

            return View(model);
        }

        public ActionResult ChangeCreateIndex()
        {
            ViewBag.Creater = _db.DataBase.Query<Account>("select * from account", null).ToDictionary(k => k.Id, v => v.NickName);
            ViewBag.Status = ((int)CBPurchaseOrderStatus.Create).ToString() + "," + ((int)CBPurchaseOrderStatus.Reject).ToString();//typeof(CBPurchaseOrderStatus).GetValueToDescription();
            ViewBag.BillType = (int)PurchaseOrderBillType.StoreOrder;
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseChange;
            return View();
        }

        public ActionResult ChangeCreate()
        {
            ViewBag.Status = CBPurchaseOrderStatus.Create.Description();
            ViewBag.CreatedByName = _context.CurrentAccount.NickName;
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseChange;
            SetUserAuthention();
            return View();
        }



        [HttpPost]
        public JsonResult ChangeCreate(PurchaseOrderModel model)
        {
            model.EditBy = _context.CurrentAccount.AccountId;
            model.EditByName = _context.CurrentAccount.NickName;
            _purchaseFacede.ChangeCreate(model);
            return Json(new { success = true, ids = model.Ids });
        }


        public ActionResult ChangeEdit(int Id)
        {
            var model = _purchaseFacede.GetById(Id);
            model.Items = model.Items.Where(n => n.Quantity > 0).ToList();
            ViewBag.PurchaseOrderItems = JsonConvert.SerializeObject(model.Items.ToArray());
            //查询处理流程：
            //var logs = _query.FindAll<ProcessHistory>(n => n.FormId == id && n.FormType == BillIdentity.PurchaseRefundOrder.ToString());
            //ViewBag.Logs = logs;

            return View(model);
        }
        [HttpPost]
        public JsonResult ChangeEdit(PurchaseOrderModel model)
        {
            model.EditBy = _context.CurrentAccount.AccountId;
            model.EditByName = _context.CurrentAccount.NickName;
            _purchaseFacede.ChangeEdit(model);
            return Json(new { success = true, ids = model.Ids });
        }



        public ActionResult ChangeAuditIndex()
        {
            ViewBag.Creater = _db.DataBase.Query<Account>("select * from account", null).ToDictionary(k => k.Id, v => v.NickName);
            ViewBag.Status = (int)CBPurchaseOrderStatus.Create;
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseChange;
            ViewBag.BillType = (int)PurchaseOrderBillType.StoreOrder;
            SetUserAuthention();
            return View();
        }

        public JsonResult Audit(string ids, string reason)
        {
            var idArray = ids.Split(',').ToIntArray().Distinct();
            foreach (var id in idArray)
            {
                _purchaseFacede.Audit(id, _context.CurrentAccount.AccountId, _context.CurrentAccount.NickName, reason);
            }

            return Json(new { success = true });
        }
        public JsonResult Reject(string ids, string reason)
        {
            var idArray = ids.Split(',').ToIntArray().Distinct();
            foreach (var id in idArray)
            {
                _purchaseFacede.Reject(id, _context.CurrentAccount.AccountId, _context.CurrentAccount.NickName, reason);
            }
            return Json(new { success = true });
        }
        public JsonResult ChangeCancel(string ids, string reason)
        {
            var idArray = ids.Split(',').ToIntArray().Distinct();
            foreach (var id in idArray)
            {
                _purchaseFacede.ChangeCancel(id, _context.CurrentAccount.AccountId, _context.CurrentAccount.NickName, reason);
            }
            return Json(new { success = true });
        }

        public ActionResult ChangeSendIndex()
        {
            ViewBag.Creater = _db.DataBase.Query<Account>("select * from account", null).ToDictionary(k => k.Id, v => v.NickName);
            ViewBag.Status = (int)CBPurchaseOrderStatus.Audited;
            ViewBag.BillType = (int)PurchaseOrderBillType.StoreOrder;
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseChange;
            return View();
        }

        public ActionResult ChangedSend(int id)
        {

            var model = _purchaseFacede.GetById(id);
            model.Items = model.Items.Where(n => n.Quantity < 0).ToList();
            ViewBag.PurchaseOrderItems = JsonConvert.SerializeObject(model.Items.ToArray());
            return View(model);
        }
        public ActionResult ChangeReceiveIndex()
        {
            ViewBag.Creater = _db.DataBase.Query<Account>("select * from account", null).ToDictionary(k => k.Id, v => v.NickName);
            ViewBag.Status = (int)CBPurchaseOrderStatus.OutStock;
            ViewBag.BillType = (int)PurchaseOrderBillType.StoreOrder;
            ViewBag.OrderType = (int)PurchaseOrderType.PurchaseChange;
            return View();
        }

        public ActionResult ChangedReceive(int id)
        {

            var model = _purchaseFacede.GetById(id);
            model.Items = model.Items.Where(n => n.Quantity > 0).ToList();
            // model.Items.OrderBy(x => x.Quantity);
            ViewBag.PurchaseOrderItems = JsonConvert.SerializeObject(model.Items.ToArray());
            return View(model);
        }

        [HttpPost]
        public JsonResult ChangeStockIn(PurchaseOrderModel model)
        {

            model.EditBy = _context.CurrentAccount.AccountId;
            model.EditByName = _context.CurrentAccount.NickName;
            _purchaseFacede.ChangeStockIn(model);
            return Json(new { success = true });
        }

        #endregion


    }
}
