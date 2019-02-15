using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Service
{
   public class ProcessHistoryService
    {
        IDBContext _db;
        public ProcessHistoryService(IDBContext dbcontext)
        {
            this._db = dbcontext;
        }

        /// <summary>
        /// 跟踪处理历史记录
        /// </summary>
        /// <param name="createdBy"></param>
        /// <param name="createdByName"></param>
        /// <param name="status"></param>
        /// <param name="formId"></param>
        /// <param name="formType"></param>
        public void Track(int createdBy, string createdByName, int status, int formId, string formType, string remark)
        {
            ProcessHistory model = new ProcessHistory()
            {
                CreatedBy = createdBy,
                CreatedByName = createdByName,
                CreatedOn = DateTime.Now,
                FormId = formId,
                FormType = formType,
                Status = status,
                Remark = remark
            };
            this._db.Insert(model);
        }
    }
}
