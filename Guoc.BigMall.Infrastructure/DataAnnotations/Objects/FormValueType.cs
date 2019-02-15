using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.DataAnnotations.Objects
{
    public enum FormValueType
    {
        //[JsonProperty("string")]
        String = 0,
        //[JsonProperty("number")]
        Number = 1,
        //[JsonProperty("boolean")]
        Boolean = 2,
        //[JsonProperty("method")]
        Method = 3,
        //[JsonProperty("regexp")]
        RegExp = 4,
        //[JsonProperty("integer")]
        Integer = 5,
        //[JsonProperty("float")]
        Float = 6,
        //[JsonProperty("array")]
        Array = 7,
        //[JsonProperty("object")]
        Object = 8,
        //[JsonProperty("enum")]
        Enum = 9,
        //[JsonProperty("date")]
        Date = 10,
        //[JsonProperty("url")]
        Url = 11,
        //[JsonProperty("hex")]
        Hex = 12,
        //[JsonProperty("email")]
        Email = 13,
    }
}
