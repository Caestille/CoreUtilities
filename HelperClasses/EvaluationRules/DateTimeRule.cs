using CoreUtilities.HelperClasses.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
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
			if (GetPropertyFunc == null)
			{
				throw new NullReferenceException("GetPropertyFunc is null");
			}

			var value = GetPropertyFunc(input);

			switch (SelectedOperation)
			{
				case AvailableOperation.EqualTo:
					return (Value1 is DateTime) ? value == (DateTime)Value1 : false;
				case AvailableOperation.NotEqualTo:
					return (Value1 is DateTime) ? value != (DateTime)Value1 : false;
				case AvailableOperation.LessThan:
					return (Value1 is DateTime) ? value < (DateTime)Value1 : false;
				case AvailableOperation.GreaterThan:
					return (Value1 is DateTime) ? value > (DateTime)Value1 : false;
				case AvailableOperation.InBetween:
					return (Value1 is DateTime && Value2 is DateTime) ? value > (DateTime)Value2 && value < (DateTime)Value1 : false;
				case AvailableOperation.OutsideOf:
					return (Value1 is DateTime && Value2 is DateTime) ? value < (DateTime)Value2 && value > (DateTime)Value1 : false;
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
			var sucess = DateTime.TryParse(value, CultureInfo.InvariantCulture, out var result);
            return sucess ? result : null;
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
