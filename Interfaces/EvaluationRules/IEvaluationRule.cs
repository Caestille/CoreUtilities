using CoreUtilities.HelperClasses.Enums;
using System;
using System.Collections.Generic;

namespace CoreUtilities.Interfaces.EvaluationRules
{
	/// <summary>
	/// An interface for classes designed to implement an evaluation step to implement.
	/// </summary>
	/// <typeparam name="TInput">The input type to be evaluated.</typeparam>
	/// <typeparam name="TEvaluate">The property type of the input to be evaluated.</typeparam>
	public interface IEvaluationRule<TInput, TEvaluate>
	{
		/// <summary>
		/// Given a <typeparamref name="TInput"/>, returns whether the object matches the set rule.
		/// </summary>
		/// <param name="input">The <typeparamref name="TInput"/> to be evaluated.</param>
		/// <returns>A <see cref="bool"/> indicating whether the given value matched the rule.</returns>
		bool Evaluate(TInput input);

		/// <summary>
		/// Configured the rule for the selected operation. Should be called when the selected operation changes. Can
		/// optionally do nothing.
		/// </summary>
		void ConfigureForSelectedOperation();

		/// <summary>
		/// Potential first value of type <typeparamref name="TEvaluate"/> to evaluate the input against according to
		/// the selected operation.
		/// </summary>
		TEvaluate? Value1 { get; }

		/// <summary>
		/// Potential second value of type <typeparamref name="TEvaluate"/> to evaluate the input against according to
		/// the selected operation.
		/// </summary>
		TEvaluate Value2 { get; }

		/// <summary>
		/// A <see cref="bool"/> indicating whether the second value is required by the selected operation. Should be
		/// set by <see cref="ConfigureForSelectedOperation"/>.
		/// </summary>
		bool Value2Usable { get; }

		/// <summary>
		/// A <see cref="Func{T, TResult}"/> accepting a value of type <typeparamref name="TInput"/> and returning a
		/// value of type <typeparamref name="TEvaluate"/>. The evaluation will use this to get the value to be
		/// evaluated from the input.
		/// </summary>
		Func<TInput, TEvaluate> GetPropertyFunc { get; }

		/// <summary>
		/// A list of available properties from the input type available to select.
		/// </summary>
		IEnumerable<string> AvailableProperties { get; }

		/// <summary>
		/// A list of evaluation operations the rule supports.
		/// </summary>
		IEnumerable<Enum> SupportedOperations { get; }

		/// <summary>
		/// The selected operation the rule will use in its evaluation.
		/// </summary>
		AvailableOperation? SelectedOperation { get; }
	}
}