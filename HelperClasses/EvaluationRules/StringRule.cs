using CoreUtilities.HelperClasses.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreUtilities.HelperClasses.EvaluationRules
{
	/// <summary>
	/// A rule operating on string values.
	/// </summary>
	/// <typeparam name="TInput">The input type to be evaluated.</typeparam>
	public class StringRule<TInput> : BaseRule<TInput, string>
	{
		/// <summary>
		/// Initialises a new <see cref="StringRule{TInput}"/>.
		/// </summary>
		/// <param name="getPropertyFunc">A <see cref="Func{T, TResult}"/> used to obtain the <see cref="string"/>
		/// value to be evaluated with from the <typeparamref name="TInput"/>.</param>
		public StringRule(Func<TInput, string> getPropertyFunc) : base(getPropertyFunc) { }

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
					throw new NotSupportedException("EqualTo rule type is not supported for value type rule");
				case AvailableOperation.NotEqualTo:
					throw new NotSupportedException("NotEqualTo rule type is not supported for value type rule");
				case AvailableOperation.LessThan:
					throw new NotSupportedException("LessThan rule type is not supported for value type rule");
				case AvailableOperation.GreaterThan:
					throw new NotSupportedException("GreaterThan rule type is not supported for value type rule");
				case AvailableOperation.InBetween:
					throw new NotSupportedException("InBetween rule type is not supported for value type rule");
				case AvailableOperation.OutsideOf:
					throw new NotSupportedException("OutsideOf rule type is not supported for value type rule");
				case AvailableOperation.Contains:
					return (Value1 is string) ? value.IndexOf((string)Value1, StringComparison.OrdinalIgnoreCase) >= 0 : false;
				case AvailableOperation.DoesNotContain:
					return (Value1 is string) ? !(value.IndexOf((string)Value1, StringComparison.OrdinalIgnoreCase) >= 0) : false;
			}

			return false;
		}

		public override string SerialiseValue(object value)
		{
			return (string)value;
		}

		public override object DeserialiseValue(string value)
		{
			return value;
		}

		/// <inheritdoc />
		public override IEnumerable<string> AvailableProperties
			=> typeof(TInput).GetProperties().Where(x => x.PropertyType == typeof(string)).Select(x => x.Name);

		/// <inheritdoc />
		public override IEnumerable<Enum> SupportedOperations => new List<Enum>()
		{
			AvailableOperation.Contains,
			AvailableOperation.DoesNotContain,
		};
	}
}
