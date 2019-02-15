using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure;

namespace Guoc.BigMall.Domain.Service
{
    public class RoleService
    {
        IDBContext _db;
        public RoleService(IDBContext dbContext)
        {
            this._db = dbContext;
        }

        public void Create(Role model)
        {
            // if (_db.Table.Exists<Role>(n => n.Name == model.Name))
            var name = model.Name;
           if (_db.Table<Role>().Exists(n => n.Name == name))
            {
                throw new FriendlyException("名称重复!");
            }
            
        }

        public void Update(Role model)
        {
            var name = model.Name;
            var id = model.Id;
            if (_db.Table<Role>().Exists(n => n.Name == name && n.Id != id))
            {
                throw new FriendlyException("名称重复!");
            }
           // var entity = _db.Table.Find<Role>(model.Id);
            var entity = _db.Table<Role>().FirstOrDefault(n=>n.Id==id);
            entity.Name = model.Name;
            entity.Description = model.Description;
            _db.Update(entity);
        }

        public void Delete(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                throw new FriendlyException("id 参数为空");
            }
            var arrIds = ids.Split(',').ToIntArray();
            foreach (var id in arrIds)
            {
                if (_db.Table<Account>().Exists(m => m.RoleId == id))
                {
                    throw new FriendlyException("存在配置该角色的用户，无法删除！");
                }
                if (_db.Table<RoleMenu>().Exists(m => m.RoleId == id))
                {
                    _db.Delete<RoleMenu>(m => m.RoleId == id);
                }
            }
            //_db.Delete<Role>(arrIds);
            _db.Delete<Role>(n => n.Id.In(arrIds));
            //删除权限
        }
    }
}
