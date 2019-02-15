using Guoc.BigMall.Admin.Services;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using Newtonsoft.Json;
using System.Collections;
using System.Linq;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
    [Permission]
    public class RoleController : Controller
    {
        IContextFacade _context;
        IDBContext _db;
        IRoleFacade _roleFacade;
        IMenuFacade _menuFacade;
        public RoleController(IContextFacade context, IDBContext db, IRoleFacade roleFacade, IMenuFacade menuFacade)
        {
            this._context = context;
            this._db = db;
            this._roleFacade = roleFacade;
            this._menuFacade = menuFacade;


        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadData(Pager page, string name)
        {
            var rows = _roleFacade.GetPageList(page, name);

            return Json(new { success = true, data = rows, total = page.Total }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(string ids)
        {
            _roleFacade.Delete(ids);
            return Json(new { success = true });
        }

        public ActionResult Create()
        {
            int roleID = _context.CurrentAccount.RoleId;
            //var items = _query.FindAll<RoleMenu>(n => n.RoleId == roleID&&n.RoleId!=1).ToList();
            //加载权限资源
            //var menus = _menuFacade.LoadMenuTree().Select(m => new { id = m.Id, pId = m.ParentId, name = m.Name }).ToList();

            var treeNew = _menuFacade.LoadMenuTreeNode();
            //if (items.Count > 0)
            //{
            //    menus = (from c in menus
            //             join s in items on c.id equals s.MenuId
            //             select new { c.id, c.pId, c.name }).ToList();
            //}


            var tree = JsonConvert.SerializeObject(treeNew);
            ViewBag.menuTree = tree;
            return View();
        }

        [HttpPost]
        public JsonResult Create(RoleModel model)
        {
            _roleFacade.Create(model);
            return Json(new { success = true });
        }


        public ActionResult Edit(int id)
        {
            var model = _db.Table<Role>().FirstOrDefault(n => n.Id == id);
            var roleMenus = _db.Table<RoleMenu>().Where(n => n.RoleId == id).Select(s => s.MenuId.ToString()).ToList();//编辑的角色拥有的权限
            ArrayList nodes = new ArrayList();
            foreach (var menuId in roleMenus)
            {
                var childNodes = _db.Table<Menu>().Where(n => n.ParentId.ToString() == menuId).Select(s => s.Id).ToList();
                if (childNodes.Count == 0 || childNodes.All(t => roleMenus.Any(m => m.Equals(t))))//所有子节点权限都有
                {
                    nodes.Add(menuId);
                }
            }
            var treeNew = _menuFacade.LoadMenuTreeNode();
            var tree = JsonConvert.SerializeObject(treeNew);
            var curentItems = JsonConvert.SerializeObject(nodes);
            ViewBag.menuTree = tree;
            ViewBag.curentItems = curentItems;
            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(RoleModel model)
        {
            _roleFacade.Edit(model);
            return Json(new { success = true });
        }
    }
}