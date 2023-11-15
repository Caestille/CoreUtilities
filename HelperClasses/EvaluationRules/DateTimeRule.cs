using CoreUtilities.HelperClasses.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreUtilities.HelperClasses.EvaluationRules
{
    /// <summary>
    /// A rule for operating on DateTime values.
    /// </summary>
    /// <typeparam name="TInput">The input type to be evaluated with.</typeparam>
    public class DateTimeRule<TInput> : BaseRule<TInput, DateTime>
	{
		/// <summary>
		/// Initialises a new <see cref="DateTimeRule{TInput}"/>.
		/// </summary>
		/// <param name="getPropertyFunc">The <see cref="Func{T, TResult}"/> used to obtain the property from the
		/// <typeparamref name="TInput"/> to be evaluated with.</param>
		public DateTimeRule(Func<TInput, DateTime> getPropertyFunc) : base(getPropertyFunc) { }

		/// <inheritdoc />
		public override bool Evaluate(TInput input)
		{
			var value = GetPropertyFunc(input);

			switch (SelectedOperation)
			{
				case AvailableOperation.EqualTo:
					return value == (DateTime)Value1;
				case AvailableOperation.NotEqualTo:
					return value != (DateTime)Value1;
				case AvailableOperation.LessThan:
					return value < (DateTime)Value1;
				case AvailableOperation.GreaterThan:
					return value > (DateTime)Value1;
				case AvailableOperation.InBetween:
					return value > (DateTime)Value2 && value < (DateTime)Value1;
				case AvailableOperation.OutsideOf:
					return value < (DateTime)Value2 && value > (DateTime)Value1;
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
			return ((DateTime)value).ToString();
		}

		public override object DeserialiseValue(string value)
		{
			return DateTime.Parse(value);
		}

		/// <inheritdoc />
		public override IEnumerable<string> AvailableProperties
			=> typeof(TInput).GetProperties().Where(x => x.PropertyType == typeof(DateTime)).Select(x => x.Name);

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
