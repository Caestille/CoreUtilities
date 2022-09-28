using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace CoreUtilities.HelperClasses
{
	public class EnumNamePair : ObservableObject
	{
		private string name;
		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

		private Enum enumValue;
		public Enum EnumValue
		{
			get => enumValue;
			set
			{
				SetProperty(ref enumValue, value);
				Name = enumValue.GetEnumDescription();
			}
		}

		public EnumNamePair(Enum enumValue)
		{
			EnumValue = enumValue;
		}
	}
}
