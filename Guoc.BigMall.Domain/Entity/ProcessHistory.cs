using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class ProcessHistory : BaseEntity
    {
        public ProcessHistory()
        {
            this.CreatedOn = DateTime.Now;
        }

        public ProcessHistory(int createdBy, string createdByName, int status, int formId, string formType, string remark)
        {
            this.CreatedBy = createdBy;
            this.CreatedByName = createdByName;
            this.Status = status;
            this.FormId = formId;
            this.FormType = formType;
            this.Remark = remark;
            this.CreatedOn = DateTime.Now;
        }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 表单Id
        /// </summary>
        public int FormId { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        public string FormType { get; set; }
        /// <summary>
        /// 操作备注
        /// </summary>
        public string Remark { get; set; }

        public string CreateSql(string tableName, string code)
        {
            string sql = @"insert into processhistory (Createdby,CreatedByName,CreatedOn,Status,FormId,FormType,Remark)
select @Createdby,@CreatedByName,@CreatedOn,@Status,Id,@FormType,@Remark from {0} where Code='{1}'";
            return string.Format(sql, tableName, code);
        }
    }
}
