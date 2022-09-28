using System.Collections.Generic;

namespace CoreUtilities.Interfaces.EvaluationRules
{
    public interface IRuleSelector<TInput>
    {
        IEnumerable<string> AvailableRuleTypes { get; }

        string SelectedRuleType { get; }

        IRuleWrapper<TInput> SelectedRule { get; }

        bool Evaluate(TInput input);
    }
}