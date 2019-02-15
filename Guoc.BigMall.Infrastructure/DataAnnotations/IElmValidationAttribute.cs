using Guoc.BigMall.Infrastructure.DataAnnotations.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.DataAnnotations
{
    public interface IElmValidationAttribute
    {
        FormValueType FormValueType { get; }
        RuleTrigger FormRuleTrigger { get; }

        Rule TransformToRule();
    }
}
