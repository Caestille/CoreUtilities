using System.Windows;

namespace CoreUtilities.HelperClasses
{
	/// <summary>
	/// Proxy for a binding, allowing nested data contexts to access a parent binding.
	/// </summary>
	public class BindingProxy : Freezable
	{
		protected override Freezable CreateInstanceCore()
		{
			return new BindingProxy();
		}

		/// <summary>
		/// The data.
		/// </summary>
		public object Data
		{
			get => GetValue(DataProperty);
			set => SetValue(DataProperty, value);
		}

		public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
			"Data",
			typeof(object),
			typeof(BindingProxy),
			new UIPropertyMetadata(null));
	}
}