namespace CoreUtilities.Interfaces.EvaluationRules
{
    public interface IRuleWrapper<TInput>
    {
        bool Evaluate(TInput input);

		string Value1 { get; set; }

		string Value2 { get; set; }

        string SelectedOperation { get; set; }
    }
}