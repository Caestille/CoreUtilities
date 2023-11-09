using CoreUtilities.HelperClasses.Enums;
using CoreUtilities.Interfaces.EvaluationRules;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CoreUtilities.HelperClasses.EvaluationRules
{
	/// <summary>
	/// A rule for operating on two <see cref="BaseRule{TInput, TEvaluate}"/> derivatives and indicating whether one or
	/// both match their set conditions.
	/// </summary>
	/// <typeparam name="TInput">The type to be given.</typeparam>
	/// <typeparam name="TEvaluate">The property type to evaluate.</typeparam>
	public class OrRule<TInput, TEvaluate> : BaseRule<TInput, TEvaluate> where TEvaluate : IRuleConfigurer<TInput>
	{
		/// <summary>
		/// Initialises a new <see cref="OrRule{TInput, TEvaluate}"/>.
		/// </summary>
		public OrRule() : base(null)
		{
			Value1 = (TEvaluate)Activator.CreateInstance(typeof(TEvaluate));
			Value2 = (TEvaluate)Activator.CreateInstance(typeof(TEvaluate));
			SelectedOperation = AvailableOperation.EqualTo;
		}

		/// <inheritdoc />
		public override bool Evaluate(TInput input)
		{
			return ((TEvaluate)Value1).Evaluate(input) || ((TEvaluate)Value1).Evaluate(input);
		}

		public override string SerialiseValue(object value)
		{
			return ((TEvaluate)value).Serialise();
		}

		public override object DeserialiseValue(string value)
		{
			return ((TEvaluate)Activator.CreateInstance(typeof(TEvaluate))).Deserialise(value);
		}

		/// <inheritdoc />
		public override IEnumerable<Enum> SupportedOperations => new List<Enum>() { };
	}
}
