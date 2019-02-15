using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.Queue
{
    public interface ISimpleQueue<T>
    {
        bool Add(T t);
    }
}
