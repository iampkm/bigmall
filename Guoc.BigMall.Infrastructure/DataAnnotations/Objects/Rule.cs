using Guoc.BigMall.Infrastructure.DataAnnotations.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.DataAnnotations.Objects
{
    public class Rule
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("trigger")]
        //[JsonConverter(typeof(StringEnumConverter))]
        public RuleTrigger Trigger { get; set; }

        [JsonProperty("required")]
        public bool? Required { get; set; }

        [JsonProperty("min")]
        public object Min { get; set; }

        [JsonProperty("max")]
        public object Max { get; set; }

        [JsonProperty("len")]
        public int? Len { get; set; }

        [JsonProperty("type")]
        //[JsonConverter(typeof(StringEnumConverter))]
        public FormValueType? Type { get; set; }

        [JsonProperty("pattern")]
        [JsonConverter(typeof(JavaScriptVariableConverter))]
        public string Pattern { get; set; }

        [JsonProperty("enum")]
        public object[] Enum { get; set; }

        [JsonProperty("validator")]
        [JsonConverter(typeof(JavaScriptVariableConverter))]
        public string Validator { get; set; }
    }
}
