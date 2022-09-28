using CoreUtilities.Interfaces.EvaluationRules;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace CoreUtilities.HelperClasses.EvaluationRules
{
	public class BaseRule<TInput, TEvaluate> : ObservableObject, IEvaluationRule<TInput, TEvaluate>
	{
		public BaseRule(Func<TInput, TEvaluate> getPropertyFunc)
		{
			GetPropertyFunc = getPropertyFunc;
		}

		public virtual bool Evaluate(TInput input)
		{
			return false;
		}

		public virtual void ConfigureForSelectedOperation() { }

		private TEvaluate? value1;
		public TEvaluate? Value1
		{
			get => value1;
			set => SetProperty(ref value1, value);
		}

		private TEvaluate? value2;
		public TEvaluate? Value2
		{
			get => value2;
			set => SetProperty(ref value2, value);
		}

		private bool value2Usable;
		public bool Value2Usable
		{
			get => value2Usable;
			set => SetProperty(ref value2Usable, value);
		}

		private Func<TInput, TEvaluate>? getPropertyFunc = null;
		public Func<TInput, TEvaluate>? GetPropertyFunc
		{
			get => getPropertyFunc;
			set => SetProperty(ref getPropertyFunc, value);
		}

		public virtual IEnumerable<string> AvailableProperties => new List<string>() { };

		private AvailableRuleOperation? selectedOperation = null;
		public AvailableRuleOperation? SelectedOperation
		{
			get => selectedOperation;
			set
			{
				SetProperty(ref selectedOperation, value);
				ConfigureForSelectedOperation();
			}
		}

		public virtual IEnumerable<Enum> SupportedOperations
			=> new List<Enum>() { };
	}
}
