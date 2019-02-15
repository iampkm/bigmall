using Guoc.BigMall.Domain.Entity;
using Guoc.BigMall.Domain.ValueObject;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Service
{
    public class BillSequenceService
    {
        IDBContext _db;
        public BillSequenceService(IDBContext dbContext)
        {
            this._db = dbContext;
        }
        /// <summary>
        /// 单据号生成算法
        /// </summary>
        /// <param name="billIdentity"></param>
        /// <returns></returns>
        public string GenerateNewCode(BillIdentity billIdentity)
        {
            // 生成一个新的 Code 序列号 
            var billId = ((int)billIdentity).ToString();
            billId = billId.Length == 2 ? billId : billId.PadRight(2, '0');
            var codeSequence = new BillSequence();
            codeSequence.Id = _db.DataBase.ExecuteScalar<int>("insert into BillSequence (GuidCode) values (@GuidCode);SELECT SCOPE_IDENTITY() as Id", codeSequence);
            var sequenceId = codeSequence.Id > 99999999 ? codeSequence.Id.ToString() : codeSequence.Id.ToString().PadLeft(8, '0');
            return billId + sequenceId;
        }

        

    }
}
