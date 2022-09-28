using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CoreUtilities.Interfaces.EvaluationRules
{
	public enum AvailableRuleOperation
	{
		[Description("=")]
		EqualTo,

		[Description("=/=")]
		NotEqualTo,

		[Description(">")]
		GreaterThan,

		[Description("<")]
		LessThan,

		[Description("> X <")]
		InBetween,

		[Description("< X >")]
		OutsideOf,

		[Description("Contains")]
		Contains,

		[Description("Does not contain")]
		DoesNotContain,
	}

	public interface IEvaluationRule<TInput, TEvaluate>
	{
		bool Evaluate(TInput input);

		void ConfigureForSelectedOperation();

		TEvaluate? Value1 { get; }

		TEvaluate Value2 { get; }

		bool Value2Usable { get; }

		Func<TInput, TEvaluate> GetPropertyFunc { get; }

		IEnumerable<string> AvailableProperties { get; }

		IEnumerable<Enum> SupportedOperations { get; }

		AvailableRuleOperation? SelectedOperation { get; }
	}
}