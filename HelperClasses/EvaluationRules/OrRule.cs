using CoreUtilities.Interfaces.EvaluationRules;
using System;
using System.Collections.Generic;

namespace CoreUtilities.HelperClasses.EvaluationRules
{
    public class OrRule<TInput, TEvaluate> : BaseRule<TInput, TEvaluate> where TEvaluate : IRuleSelector<TInput>
    {
        public OrRule() : base(null)
        {
            Value1 = (TEvaluate)Activator.CreateInstance(typeof(TEvaluate));
            Value2 = (TEvaluate)Activator.CreateInstance(typeof(TEvaluate));
            SelectedOperation = AvailableRuleOperation.EqualTo;
        }

        public override bool Evaluate(TInput input)
        {
            return Value1.Evaluate(input) || Value2.Evaluate(input);
        }

        public override IEnumerable<Enum> SupportedOperations => new List<Enum>() { };
    }
}
