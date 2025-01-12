namespace CoreUtilities.Helpers.WPF;

using System.Windows;

/// <summary>
/// Proxy for a binding, allowing nested data contexts to access a parent binding.
/// </summary>
public class BindingProxy : Freezable
{
    public BindingProxy() { }

    public BindingProxy(object data)
    {
        this.Data = data;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }

    /// <summary>
    /// The data.
    /// </summary>
    public object Data
    {
        get => this.GetValue(DataProperty);
        set => this.SetValue(DataProperty, value);
    }

    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data),
        typeof(object),
        typeof(BindingProxy),
        new UIPropertyMetadata(null));
}