using CoreUtilities.HelperClasses.Enums;
using CoreUtilities.Interfaces.EvaluationRules;
using System;
using System.Collections.Generic;

namespace CoreUtilities.HelperClasses.EvaluationRules
{
	/// <summary>
	/// A rule for operating on two <see cref="BaseRule{TInput, TEvaluate}"/> derivatives and indicating whether
	/// both match their set conditions.
	/// </summary>
	/// <typeparam name="TInput">The type to be given.</typeparam>
	/// <typeparam name="TEvaluate">The property type to evaluate.</typeparam>
	public class AndRule<TInput, TEvaluate> : BaseRule<TInput, TEvaluate> where TEvaluate : IRuleConfigurer<TInput>
	{
		/// <summary>
		/// Initialises a new <see cref="AndRule{TInput, TEvaluate}"/>
		/// </summary>
		public AndRule() : base(null)
		{
			Value1 = (TEvaluate)Activator.CreateInstance(typeof(TEvaluate));
			Value2 = (TEvaluate)Activator.CreateInstance(typeof(TEvaluate));
			SelectedOperation = AvailableOperation.EqualTo;
		}

		/// <inheritdoc />
		public override bool Evaluate(TInput input)
		{
			return Value1.Evaluate(input) && Value2.Evaluate(input);
		}

		/// <inheritdoc />
		public override IEnumerable<Enum> SupportedOperations => new List<Enum>() { };
	}
}
