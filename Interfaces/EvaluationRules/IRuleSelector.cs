using CoreUtilities.HelperClasses.EvaluationRules;
using System.Collections.Generic;

namespace CoreUtilities.Interfaces.EvaluationRules
{
    /// <summary>
    /// An interface for classes configuring rules to inherit. The base 
    /// <see cref="AndRule{TInput, TEvaluate}"/> and <see cref="OrRule{TInput, TEvaluate}"/>s expect a class inheriting
    /// this as their <see cref="TEvaluate"/>.
    /// </summary>
    /// <typeparam name="TInput">The input type to be evaluated.</typeparam>
    public interface IRuleConfigurer<TInput>
    {
        /// <summary>
        /// List of available properties to evaluate against to select from.
        /// </summary>
        IEnumerable<string> EvaluationOptions { get; }

        /// <summary>
        /// The selected property to evaluate against.
        /// </summary>
        string SelectedEvaluationOption { get; }

        /// <summary>
        /// The selected rule to evaluate with.
        /// </summary>
        IRuleImplementation<TInput> SelectedRule { get; }

        /// <summary>
        /// The evaluation method.
        /// </summary>
        /// <param name="input">The input to evaluate.</param>
        /// <returns>A <see cref="bool"/> indicating whether the give property matched the configured rule.</returns>
        bool Evaluate(TInput input);
    }
}