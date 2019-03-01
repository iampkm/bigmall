using Guoc.BigMall.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Guoc.BigMall.Api.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 获取单个值
        /// </summary>
        /// <param name="id">id 参数</param>
        /// <returns></returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="LoginModel">login input uname </param>     
        /// <returns></returns>
        [Permission]
        [HttpPost]
        [Route("values/login")]
        public LoginModel Login(LoginModel model)
        {
            model.UserName += "-to-server";
            model.Password += "-to-server";
            return model;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

    /// <summary>
    ///  login model
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        ///  user name
        /// </summary>
         public string UserName { get; set; }
        /// <summary>
        ///  password
        /// </summary>
        public string Password { get; set; }
    }
}
