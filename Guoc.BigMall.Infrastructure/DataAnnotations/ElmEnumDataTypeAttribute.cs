using Guoc.BigMall.Infrastructure.DataAnnotations.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ElmEnumDataTypeAttribute : DataTypeAttribute, IElmValidationAttribute
    {
        private EnumDataTypeAttribute innerAttribute;
        public FormValueType FormValueType { get; set; }
        public RuleTrigger FormRuleTrigger { get; set; }
        public object[] EnumValues { get; set; }

        public ElmEnumDataTypeAttribute(Type enumType)
            : base(enumType.FullName)
        {
            innerAttribute = new EnumDataTypeAttribute(enumType);
        }

        public ElmEnumDataTypeAttribute(params object[] enumValues)
            : base(DataType.Custom)
        {
            if (enumValues.Length == 0)
                throw new ArgumentNullException("enumValues", "应至少包含一个值！");
            this.EnumValues = enumValues;
        }

        public override bool IsValid(object value)
        {
            if (innerAttribute != null)
                return innerAttribute.IsValid(value);

            return this.EnumValues.Any(enumValue => enumValue.Equals(value));
        }

        public Rule TransformToRule()
        {
            object[] enumValues = innerAttribute == null ? this.EnumValues : innerAttribute.EnumType.GetFields().Select(f => f.Name).ToArray();
            return new Rule()
            {
                Enum = enumValues,
                Type = this.FormValueType,
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };
        }
    }
}
