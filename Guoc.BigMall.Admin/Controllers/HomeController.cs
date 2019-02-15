using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
     [Permission]
    public class HomeController : Controller
    {
        IContextFacade _context;
        IMenuFacade _menuFacade;
        IDBContext _db;
        public HomeController(IContextFacade context, IMenuFacade menuFacade, IDBContext db)
        {
            this._context = context;
            this._menuFacade = menuFacade;
            this._db = db;

        }
        public ActionResult Index()
        {
            if (ModelState.IsValid)
            {

            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult DashBoard()
        {
            ViewBag.UserName = _context.CurrentAccount.NickName;
            ViewBag.RoleName = _context.CurrentAccount.RoleName;
            //ViewBag.StoreName = string.IsNullOrEmpty(_context.CurrentAccount.StoreName) ? "总公司" : _context.CurrentAccount.StoreName;
            //ViewBag.StoreName = _context.CurrentAccount.StoreId == 0 ? _context.CurrentAccount.RoleName : _context.CurrentAccount.StoreName;
            ViewBag.StoreName = _context.CurrentAccount.CanViewStores == null ? _context.CurrentAccount.RoleName : _context.CurrentAccount.StoreName;
            ViewBag.ButoonMenus = this.LoadButtomMenus();
            //var purchaseOrderInfo = _dashBoardQuery.GetPurchaseOrderInfo(_context.CurrentAccount.CanViewStores);
            //ViewBag.PurchaseOrderInfo = purchaseOrderInfo;
            //ViewBag.SaleCount = _dashBoardQuery.GetTodaySaleOrderCount(_context.CurrentAccount.CanViewStores);
            return View();
        }
        private string LoadButtomMenus()
        {
            var menus = _menuFacade.LoadMenu(_context.CurrentAccount.RoleId).Where(n => n.UrlType == (int)Domain.ValueObject.MenuUrlType.ButtonLink && n.ParentId != 0);
            if (_context.CurrentAccount.RoleId == 1)
            {
                //menus = _query.FindAll<Menu>(m => m.UrlType == Domain.ValueObject.MenuUrlType.ButtonLink && m.ParentId != 0);
                menus = _db.Table<Menu>().Where(m => m.UrlType == (int)Domain.ValueObject.MenuUrlType.ButtonLink && m.ParentId != 0).ToList();
            }
            StringBuilder str = new StringBuilder();
            //var purchaseOrderInfo = _dashBoardQuery.GetPurchaseOrderInfo(_context.CurrentAccount.CanViewStores);
            //ViewBag.PurchaseOrderInfo = purchaseOrderInfo;
            //ViewBag.SaleCount = _dashBoardQuery.GetTodaySaleOrderCount(_context.CurrentAccount.CanViewStores);
            //foreach (var item in menus)
            //{
            //    if (item.Url == "/SaleOrder/Index")
            //    {
            //        str.AppendFormat("<div class=\"col-sm-3 col-xs-6\" > <div class=\"small-box bg-aqua\">  <div class=\"inner\"> <h3>{0}</h3><p>今日销售单数</p> </div>  <div class=\"icon\">  <i class=\"fa fa-shopping-cart\"></i></div>  <a href=\"javascript:showTab('销售订单','/SaleOrder/Index')\" class=\"small-box-footer\">查看详细<i class=\"fa fa-arrow-circle-right\"></i></a> </div></div>", ViewBag.SaleCount);
            //        continue;
            //    }
            //    if (item.Url == "/PurchaseOrder/WaitPayIndex")
            //    {
            //        str.AppendFormat("<div class=\"col-sm-3 col-xs-6\"> <div class=\"small-box bg-yellow\"><div class=\"inner\">  <h3>{0}</h3><p>待付款采购单</p></div> <div class=\"icon\"> <i class=\"fa fa-list\"></i> </div> <a href=\"javascript:showTab('采购单-待支付','/PurchaseOrder/WaitPayIndex')\" class=\"small-box-footer\">查看详细<i class=\"fa fa-arrow-circle-right\"></i></a> </div> </div>", ViewBag.PurchaseOrderInfo.WaitPay);
            //        continue;
            //    }
            //    if (item.Url == "/PurchaseOrder/WaitReceiveIndex")
            //    {
            //        str.AppendFormat("   <div class=\"col-sm-3 col-xs-6\"> <!-- small box --><div class=\"small-box bg-green\"> <div class=\"inner\"> <h3>{0}</h3> <p>待收货采购单</p> </div> <div class=\"icon\"> <i class=\"fa fa-list\"></i> </div> <a href=\"javascript:showTab('采购单-收货入库','/PurchaseOrder/WaitReceiveIndex')\" class=\"small-box-footer\">查看详细<i class=\"fa fa-arrow-circle-right\"></i></a> </div> </div>", ViewBag.PurchaseOrderInfo.WaitReceive);
            //        continue;
            //    }
            //    if (item.Url == "/PurchaseOrder/WaitRefundIndex")
            //    {
            //        str.AppendFormat(" <div class=\"col-sm-3 col-xs-6\">  <!-- small box --> <div class=\"small-box bg-red\"> <div class=\"inner\"><h3>{0}</h3> <p>待出库退货单</p> </div> <div class=\"icon\"> <i class=\"fa fa-list\"></i>  </div> <a href=\"javascript:showTab('采购退单-退货出库','/PurchaseOrder/WaitRefundIndex')\" class=\"small-box-footer\">查看详细<i class=\"fa fa-arrow-circle-right\"></i></a> </div> </div>", ViewBag.PurchaseOrderInfo.WaitRefund);
            //    }
            //}

            return str.ToString();

        }
        private string loadMenu()
        {
            // load menu,加载当前用户的
            //  
            var menus = _menuFacade.LoadMenu(_context.CurrentAccount.RoleId).Where(n => n.UrlType == (int)Domain.ValueObject.MenuUrlType.MenuLink);
            if (!menus.Any())
            {
                //没有分配菜单，加载所有
                //menus = _query.FindAll<Menu>(m => m.UrlType == Domain.ValueObject.MenuUrlType.MenuLink);
                menus = _db.Table<Menu>().Where(m => m.UrlType ==(int)Domain.ValueObject.MenuUrlType.MenuLink).ToList();
            }
            // var menus = _query.FindAll<Menu>(m => m.UrlType == Domain.ValueObject.MenuUrlType.MenuLink);
            StringBuilder builder = new StringBuilder();

            foreach (var topMenu in menus.Where(m => m.ParentId == 0).OrderBy(n => n.DisplayOrder).ToList())
            {
                builder.AppendFormat("<li class=\"treeview\"><a href=\"javascript:showTab('{0}','{1}')\"><i class=\"fa {2}\"></i><span>{0}</span><span class=\"pull-right-container\"><i class=\"fa fa-angle-left pull-right\"></i></span></a>", topMenu.Name, topMenu.Url, topMenu.Icon);
                AddChildMenu(builder, topMenu, menus);
                builder.Append("</li>");
            }
            return builder.ToString();
        }

        public ActionResult MenuTree()
        {
            ViewBag.Menus = loadMenu();
            return PartialView();
        }

        public void AddChildMenu(StringBuilder builder, Menu parentMenu, IEnumerable<Menu> menus)
        {
            var children = menus.Where(m => m.ParentId == parentMenu.Id).OrderBy(n => n.DisplayOrder).ToList();
            if (children.Count() == 0) { return; }
            builder.Append("<ul class=\"treeview-menu\">");
            foreach (var child in children)
            {
                builder.AppendFormat("<li><a href=\"javascript:showTab('{0}','{1}')\" ><i class=\"fa fa-circle-o\"></i>{0}</a>", child.Name, child.Url);
                AddChildMenu(builder, child, menus);
                builder.Append("</li>");
            }
            builder.Append("</ul>");
        }

        private void loadError()
        {
            throw new Exception("code is error");
        }
    }
}