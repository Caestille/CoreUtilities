using CoreUtilities.Interfaces.EvaluationRules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreUtilities.HelperClasses.EvaluationRules
{
    public class StringRule<TInput> : BaseRule<TInput, string>
    {
        public StringRule(Func<TInput, string> getPropertyFunc) : base(getPropertyFunc) { }
        public override bool Evaluate(TInput input)
        {
            var value = GetPropertyFunc(input);

            switch (SelectedOperation)
            {
                case AvailableRuleOperation.EqualTo:
                    throw new NotSupportedException("EqualTo rule type is not supported for value type rule");
				case AvailableRuleOperation.NotEqualTo:
					throw new NotSupportedException("NotEqualTo rule type is not supported for value type rule");
				case AvailableRuleOperation.LessThan:
                    throw new NotSupportedException("LessThan rule type is not supported for value type rule");
                case AvailableRuleOperation.GreaterThan:
                    throw new NotSupportedException("GreaterThan rule type is not supported for value type rule");
                case AvailableRuleOperation.InBetween:
                    throw new NotSupportedException("InBetween rule type is not supported for value type rule");
                case AvailableRuleOperation.OutsideOf:
                    throw new NotSupportedException("OutsideOf rule type is not supported for value type rule");
                case AvailableRuleOperation.Contains:
                    return value.IndexOf(Value1, StringComparison.OrdinalIgnoreCase) >= 0;
                case AvailableRuleOperation.DoesNotContain:
                    return !(value.IndexOf(Value1, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return false;
        }

        public virtual IEnumerable<string> AvailableProperties
            => typeof(TInput).GetProperties().Where(x => x.PropertyType == typeof(string)).Select(x => x.Name);

        public override IEnumerable<Enum> SupportedOperations => new List<Enum>()
        {
            AvailableRuleOperation.Contains,
            AvailableRuleOperation.DoesNotContain,
        };
    }
}
