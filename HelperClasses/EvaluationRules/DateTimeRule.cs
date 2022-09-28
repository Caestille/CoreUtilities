using CoreUtilities.Interfaces.EvaluationRules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreUtilities.HelperClasses.EvaluationRules
{
    public class DateTimeRule<TInput> : BaseRule<TInput, DateTime>
    {
        public DateTimeRule(Func<TInput, DateTime> getPropertyFunc) : base(getPropertyFunc) { }

        public override bool Evaluate(TInput input)
        {
            var value = GetPropertyFunc(input);

            switch (SelectedOperation)
            {
                case AvailableRuleOperation.EqualTo:
                    return value == Value1;
				case AvailableRuleOperation.NotEqualTo:
					return value != Value1;
				case AvailableRuleOperation.LessThan:
                    return value < Value1;
                case AvailableRuleOperation.GreaterThan:
                    return value > Value1;
                case AvailableRuleOperation.InBetween:
                    return value > Value2 && value < Value1;
                case AvailableRuleOperation.OutsideOf:
                    return value < Value2 && value > Value1;
                case AvailableRuleOperation.Contains:
                    throw new NotSupportedException("Contains rule type is not supported for value type rule");
                case AvailableRuleOperation.DoesNotContain:
                    throw new NotSupportedException("DoesNotContain rule type is not supported for value type rule");
            }

            return false;
        }

        public override void ConfigureForSelectedOperation()
        {
            switch (SelectedOperation)
            {
                case AvailableRuleOperation.EqualTo:
                    Value2Usable = false;
                    break;
				case AvailableRuleOperation.NotEqualTo:
					Value2Usable = false;
					break;
				case AvailableRuleOperation.LessThan:
                    Value2Usable = false;
                    break;
                case AvailableRuleOperation.GreaterThan:
                    Value2Usable = false;
                    break;
                case AvailableRuleOperation.InBetween:
                    Value2Usable = true;
                    break;
                case AvailableRuleOperation.OutsideOf:
                    Value2Usable = true;
                    break;
                case AvailableRuleOperation.Contains:
                    throw new NotSupportedException("Contains rule type is not supported for value type rule");
                case AvailableRuleOperation.DoesNotContain:
                    throw new NotSupportedException("DoesNotContain rule type is not supported for value type rule");
            }
            base.ConfigureForSelectedOperation();
        }

        public virtual IEnumerable<string> AvailableProperties
            => typeof(TInput).GetProperties().Where(x => x.PropertyType == typeof(DateTime)).Select(x => x.Name);

        public override IEnumerable<Enum> SupportedOperations => new List<Enum>()
        {
            AvailableRuleOperation.EqualTo,
			AvailableRuleOperation.NotEqualTo,
			AvailableRuleOperation.GreaterThan,
            AvailableRuleOperation.LessThan,
            AvailableRuleOperation.InBetween,
            AvailableRuleOperation.OutsideOf,
        };
    }
}
