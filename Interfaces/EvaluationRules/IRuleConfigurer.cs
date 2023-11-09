using CoreUtilities.HelperClasses.EvaluationRules;

namespace CoreUtilities.Interfaces.EvaluationRules
{
	/// <summary>
	/// An interface for classes configuring rules to inherit. The base 
	/// <see cref="AndRule{TInput, TEvaluate}"/> and <see cref="OrRule{TInput, TEvaluate}"/>s expect a class inheriting
	/// this as their <see cref="TEvaluate"/>.
	/// </summary>
	/// <typeparam name="TInput">The input type to be evaluated.</typeparam>
	public interface IRuleConfigurer<TInput>
	{
		/// <summary>
		/// The selected rule to evaluate with.
		/// </summary>
		IRule<TInput> SelectedRule { get; }

		/// <summary>
		/// The evaluation method.
		/// </summary>
		/// <param name="input">The input to evaluate.</param>
		/// <returns>A <see cref="bool"/> indicating whether the give property matched the configured rule.</returns>
		bool Evaluate(TInput input);

		/// <summary>
		/// Serialises the instance.
		/// </summary>
		/// <returns>The serialised instance.</returns>
		string Serialise();

		/// <summary>
		/// Deserialises a representative string to an instance.
		/// </summary>
		/// <param name="input">The input string to deserialise.</param>
		/// <returns>A deserialised object.</returns>
		object Deserialise(string input);
	}
}