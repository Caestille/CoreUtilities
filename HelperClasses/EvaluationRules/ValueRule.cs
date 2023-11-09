using CoreUtilities.HelperClasses.Enums;
using CoreUtilities.Interfaces.EvaluationRules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreUtilities.HelperClasses.EvaluationRules
{
	/// <summary>
	/// A rule operating on numerical values.
	/// </summary>
	/// <typeparam name="TInput">The input type to be evaluated.</typeparam>
	public class ValueRule<TInput> : BaseRule<TInput, double?>
	{
		/// <summary>
		/// Initialises a new <see cref="ValueRule{TInput}"/>, accepts a <see cref="Func{T, TResult}"/> which given a
		/// <typeparamref name="TInput"/>, returns a <see cref="double"/> which is the value to be evaluated with.
		/// </summary>
		/// <param name="getPropertyFunc"></param>
		public ValueRule(Func<TInput, double?> getPropertyFunc) : base(getPropertyFunc) { }

		/// <inheritdoc />
		public override bool Evaluate(TInput input)
		{
			var value = GetPropertyFunc(input);

			if (value == null) return false;

			switch (SelectedOperation)
			{
				case AvailableOperation.EqualTo:
					return value == (double)Value1;
				case AvailableOperation.NotEqualTo:
					return value != (double)Value1;
				case AvailableOperation.LessThan:
					return value < (double)Value1;
				case AvailableOperation.GreaterThan:
					return value > (double)Value1;
				case AvailableOperation.InBetween:
					return value > (double)Value2 && value < (double)Value1;
				case AvailableOperation.OutsideOf:
					return value < (double)Value2 && value > (double)Value1;
				case AvailableOperation.Contains:
					throw new NotSupportedException("Contains rule type is not supported for value type rule");
				case AvailableOperation.DoesNotContain:
					throw new NotSupportedException("DoesNotContain rule type is not supported for value type rule");
			}

			return false;
		}

		/// <inheritdoc />
		public override void ConfigureForSelectedOperation()
		{
			switch (SelectedOperation)
			{
				case AvailableOperation.EqualTo:
					Value2Usable = false;
					break;
				case AvailableOperation.NotEqualTo:
					Value2Usable = false;
					break;
				case AvailableOperation.LessThan:
					Value2Usable = false;
					break;
				case AvailableOperation.GreaterThan:
					Value2Usable = false;
					break;
				case AvailableOperation.InBetween:
					Value2Usable = true;
					break;
				case AvailableOperation.OutsideOf:
					Value2Usable = true;
					break;
				case AvailableOperation.Contains:
					throw new NotSupportedException("Contains rule type is not supported for value type rule");
				case AvailableOperation.DoesNotContain:
					throw new NotSupportedException("DoesNotContain rule type is not supported for value type rule");
			}
			base.ConfigureForSelectedOperation();
		}

		public override string SerialiseValue(object value)
		{
			return ((double)value).ToString();
		}

		public override object DeserialiseValue(string value)
		{
			return double.Parse(value);
		}

		/// <inheritdoc />
		public override IEnumerable<string> AvailableProperties
			=> typeof(TInput).GetProperties().Where(x => x.PropertyType == typeof(double)).Select(x => x.Name);

		/// <inheritdoc />
		public override IEnumerable<Enum> SupportedOperations => new List<Enum>()
		{
			AvailableOperation.EqualTo,
			AvailableOperation.NotEqualTo,
			AvailableOperation.GreaterThan,
			AvailableOperation.LessThan,
			AvailableOperation.InBetween,
			AvailableOperation.OutsideOf,
		};
	}
}
