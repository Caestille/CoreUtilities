namespace CoreUtilities.Helpers.EvaluationRules;

using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreUtilities.Helpers.Enums;
using CoreUtilities.Interfaces.EvaluationRules;

/// <summary>
/// The base rule for specific value rule types to inherit. Defines some standard behaviour and properties.
/// </summary>
/// <typeparam name="TInput">The input type for the rule to expect.</typeparam>
/// <typeparam name="TEvaluate">The value type the rule will evaluate with.</typeparam>
public abstract class BaseRule<TInput, TEvaluate> : ObservableObject, IRule<TInput>
{
    private object? value1;
    private object? value2;
    private bool value2Usable;
    private Func<TInput, TEvaluate>? getPropertyFunc;
    private AvailableOperation? selectedOperation = null;

    /// <summary>
    /// Initialises a new <see cref="BaseRule{TInput, TEvaluate}"/>.
    /// </summary>
    /// <param name="getPropertyFunc">The <see cref="Func{T, TResult}"/> used the obtain the
    /// <typeparamref name="TEvaluate"/> to evaluate with from the given <typeparamref name="TInput"/>.</param>
    public BaseRule(Func<TInput, TEvaluate>? getPropertyFunc)
    {
        this.getPropertyFunc = getPropertyFunc;
    }

    /// <inheritdoc />
    public object? Value1
    {
        get => this.value1;
        set => this.SetProperty(ref this.value1, value);
    }

    /// <inheritdoc />
    public object? Value2
    {
        get => this.value2;
        set => this.SetProperty(ref this.value2, value);
    }

    /// <inheritdoc />
    public bool Value2Usable
    {
        get => this.value2Usable;
        set => this.SetProperty(ref this.value2Usable, value);
    }

    /// <inheritdoc />
    public Func<TInput, TEvaluate>? GetPropertyFunc
    {
        get => this.getPropertyFunc;
        set => this.SetProperty(ref this.getPropertyFunc, value);
    }

    /// <inheritdoc />
    public virtual IEnumerable<string> AvailableProperties => new List<string>() { };

    /// <inheritdoc />
    public AvailableOperation? SelectedOperation
    {
        get => this.selectedOperation;
        set
        {
            this.SetProperty(ref this.selectedOperation, value);
            this.ConfigureForSelectedOperation();
        }
    }

    /// <inheritdoc />
    public virtual IEnumerable<Enum> SupportedOperations => new List<Enum>() { };

    /// <inheritdoc />
    public virtual bool Evaluate(TInput input) => false;

    /// <inheritdoc />
    public virtual void ConfigureForSelectedOperation() { }

    public abstract string SerialiseValue(object value);

    public abstract object? DeserialiseValue(string value);
}
