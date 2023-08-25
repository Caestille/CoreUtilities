using CoreUtilities.HelperClasses.EvaluationRules;

namespace CoreUtilities.Interfaces.EvaluationRules
{
	/// <summary>
	/// Generic interface for classes implementing any of <see cref="DateTimeRule{TInput}"/>,
	/// <see cref="StringRule{TInput}"/> <see cref="ValueRule{TInput}"/>, <see cref="OrRule{TInput, TEvaluate}"/> or
	/// <see cref="AndRule{TInput, TEvaluate}"/>.
	/// </summary>
	/// <typeparam name="TInput">The input type to be evaluated</typeparam>
	public interface IRuleImplementation<TInput>
	{
		/// <summary>
		/// Executes the rule on a given input, and returns a <see cref="bool"/> indicating whether the value matched
		/// the rule or not
		/// </summary>
		/// <param name="input">The input to be evaluated</param>
		/// <returns></returns>
		bool Evaluate(TInput input);

		/// <summary>
		/// Gets the potential first value of the rule. Depending on the inheritor, this may not be set
		/// </summary>
		string Value1 { get; set; }

		/// <summary>
		/// Gets the potential second value of the rule. Depending on the inheritor, this may not be set
		/// </summary>
		string Value2 { get; set; }

		/// <summary>
		/// The rule operation/comparison to be executed (e.g.: Less than, more than etc).
		/// </summary>
		string SelectedOperation { get; set; }
	}
}