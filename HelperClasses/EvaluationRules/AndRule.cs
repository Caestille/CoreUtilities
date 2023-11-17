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
	public class AndRule<TInput, TEvaluate> : BaseRule<TInput, TEvaluate> where TEvaluate : notnull, IRuleConfigurer<TInput>
	{
		/// <summary>
		/// Initialises a new <see cref="AndRule{TInput, TEvaluate}"/>
		/// </summary>
		public AndRule() : base(null)
		{
			Value1 = Activator.CreateInstance(typeof(TEvaluate));
			Value2 = Activator.CreateInstance(typeof(TEvaluate));
			SelectedOperation = AvailableOperation.EqualTo;
		}

		/// <inheritdoc />
		public override bool Evaluate(TInput input)
		{
			if (Value1 is TEvaluate val1 && Value2 is TEvaluate val2)
			{
				return val1.Evaluate(input) && val2.Evaluate(input);
			}

			return false;
		}

		public override string SerialiseValue(object value)
		{
			if (value is TEvaluate val)
			{
				return val.Serialise();
			}

			return string.Empty;
		}

		public override object DeserialiseValue(string value)
		{
			return ((TEvaluate)Activator.CreateInstance(typeof(TEvaluate))!).Deserialise(value);
		}

		/// <inheritdoc />
		public override IEnumerable<Enum> SupportedOperations => new List<Enum>() { };
	}
}
