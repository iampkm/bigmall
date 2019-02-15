using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.Extension
{

    /// <typeparam name="T">Type of the cloneable objects.</typeparam>  
    [Serializable]
    public class CloneBase<T> : ICloneable<T> where T : CloneBase<T>
    {
        /// <summary>  
        /// Creates a new object that is cloned from the current instance.  
        /// </summary>  
        /// <returns>  
        /// A new object that is cloned from the current instance.  
        /// </returns>  
        public T Clone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream) as T;
        }

        /// <summary>  
        /// Creates a new object that is a copy of the current instance.  
        /// </summary>  
        /// <returns>  
        /// A new object that is a copy of this instance. (return in the type of object)  
        /// </returns>  
        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
