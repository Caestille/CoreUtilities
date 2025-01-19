namespace CoreUtilities.Helpers.EvaluationRules;

using System;
using System.Collections.Generic;
using CoreUtilities.Helpers.Enums;
using CoreUtilities.Interfaces.EvaluationRules;

/// <summary>
/// A rule for operating on two <see cref="BaseRule{TInput, TEvaluate}"/> derivatives and indicating whether one or
/// both match their set conditions.
/// </summary>
/// <typeparam name="TInput">The type to be given.</typeparam>
/// <typeparam name="TEvaluate">The property type to evaluate.</typeparam>
public class OrRule<TInput, TEvaluate> : BaseRule<TInput, TEvaluate>
    where TEvaluate : IRuleConfigurer<TInput>
{
    /// <summary>
    /// Initialises a new <see cref="OrRule{TInput, TEvaluate}"/>.
    /// </summary>
    public OrRule()
        : base(null)
    {
        this.Value1 = Activator.CreateInstance<TEvaluate>();
        this.Value2 = Activator.CreateInstance<TEvaluate>();
        this.SelectedOperation = AvailableOperation.EqualTo;
    }

    /// <inheritdoc />
    public override IEnumerable<Enum> SupportedOperations => new List<Enum>() { };

    /// <inheritdoc />
    public override bool Evaluate(TInput input)
    {
        if (this.Value1 is TEvaluate val1 && this.Value2 is TEvaluate val2)
        {
            return val1.Evaluate(input) || val2.Evaluate(input);
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

    public override object DeserialiseValue(string value) => Activator.CreateInstance<TEvaluate>()!.Deserialise(value);
}
