namespace CoreUtilities.Helpers.EvaluationRules;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CoreUtilities.Helpers.Enums;

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
    public DateTimeRule(Func<TInput, DateTime> getPropertyFunc)
        : base(getPropertyFunc) { }

    /// <inheritdoc />
    public override IEnumerable<string> AvailableProperties
        => typeof(TInput)
            .GetProperties()
            .Where(x => x.PropertyType == typeof(DateTime))
            .Select(x => x.Name);

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

    /// <inheritdoc />
    public override bool Evaluate(TInput input)
    {
        if (this.GetPropertyFunc == null)
        {
            throw new NullReferenceException("GetPropertyFunc is null");
        }

        var value = this.GetPropertyFunc(input);

        switch (this.SelectedOperation)
        {
            case AvailableOperation.EqualTo:
                return this.Value1 is DateTime ? value == (DateTime)this.Value1 : false;
            case AvailableOperation.NotEqualTo:
                return this.Value1 is DateTime ? value != (DateTime)this.Value1 : false;
            case AvailableOperation.LessThan:
                return this.Value1 is DateTime ? value < (DateTime)this.Value1 : false;
            case AvailableOperation.GreaterThan:
                return this.Value1 is DateTime ? value > (DateTime)this.Value1 : false;
            case AvailableOperation.InBetween:
                return this.Value1 is DateTime && this.Value2 is DateTime ? value > (DateTime)this.Value2 && value < (DateTime)this.Value1 : false;
            case AvailableOperation.OutsideOf:
                return this.Value1 is DateTime && this.Value2 is DateTime ? value < (DateTime)this.Value2 && value > (DateTime)this.Value1 : false;
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
        switch (this.SelectedOperation)
        {
            case AvailableOperation.EqualTo:
                this.Value2Usable = false;
                break;
            case AvailableOperation.NotEqualTo:
                this.Value2Usable = false;
                break;
            case AvailableOperation.LessThan:
                this.Value2Usable = false;
                break;
            case AvailableOperation.GreaterThan:
                this.Value2Usable = false;
                break;
            case AvailableOperation.InBetween:
                this.Value2Usable = true;
                break;
            case AvailableOperation.OutsideOf:
                this.Value2Usable = true;
                break;
            case AvailableOperation.Contains:
                throw new NotSupportedException("Contains rule type is not supported for value type rule");
            case AvailableOperation.DoesNotContain:
                throw new NotSupportedException("DoesNotContain rule type is not supported for value type rule");
        }

        base.ConfigureForSelectedOperation();
    }

    public override string SerialiseValue(object value) => ((DateTime)value).ToString();

    public override object? DeserialiseValue(string value)
    {
        var success = DateTime.TryParse(value, CultureInfo.InvariantCulture, out var result);
        return success ? result : null;
    }
}
