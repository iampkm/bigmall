using Guoc.BigMall.Infrastructure.DataAnnotations.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ElmRangeAttribute : RangeAttribute, IElmValidationAttribute
    {
        public FormValueType FormValueType { get; private set; }
        public RuleTrigger FormRuleTrigger { get; set; }

        public ElmRangeAttribute(double minimum, double maximum)
            : base(minimum, maximum)
        {
            this.FormValueType = FormValueType.Number;
        }

        public ElmRangeAttribute(int minimum, int maximum)
            : base(minimum, maximum)
        {
            this.FormValueType = FormValueType.Integer;
        }

        public ElmRangeAttribute(Type type, string minimum, string maximum)
            : base(type, minimum, maximum)
        {
            if (type == typeof(DateTime))
            {
                this.FormValueType = FormValueType.Date;
            }
            else if (type == typeof(char))
            {
                if (minimum.Length != 1 || maximum.Length != 1)
                    throw new ArgumentException("传入参数和类型不匹配。", "minimum");
                this.FormValueType = FormValueType.String;
            }
            else if (type == typeof(string))
            {
                this.FormValueType = FormValueType.String;//验证字符串长度范围
            }
            else if (type == typeof(int) || type == typeof(short) || type == typeof(long))
            {
                this.FormValueType = FormValueType.Integer;
            }
            else if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
            {
                this.FormValueType = FormValueType.Number;
            }
            else
            {
                throw new ArgumentException("不支持的验证类型。", "type");
            }
        }

        public Rule TransformToRule()
        {
            var rule = new Rule()
            {
                Type = this.FormValueType,
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };

            if (this.OperandType == typeof(char))
            {
                rule.Pattern = string.Format("/^[{0}-{1}]$/", this.Minimum, this.Maximum);
                return rule;
            }

            rule.Min = this.Minimum;
            rule.Max = this.Maximum;
            return rule;
        }
    }
}
