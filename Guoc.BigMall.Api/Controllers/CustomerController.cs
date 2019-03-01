using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Guoc.BigMall.Api.Controllers
{
    public class CustomerController : ApiController
    {
        /// <summary>
        /// get one customer
        /// </summary>
        /// <param name="id">only id</param>
        /// <returns>reutrn id result</returns>
        public string get(int id)
        {
            return id.ToString();
        }
    }
}
