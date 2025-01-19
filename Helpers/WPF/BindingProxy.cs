namespace CoreUtilities.Helpers.WPF;

using System.Windows;

/// <summary>
/// Proxy for a binding, allowing nested data contexts to access a parent binding.
/// </summary>
public class BindingProxy : Freezable
{
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data),
        typeof(object),
        typeof(BindingProxy),
        new UIPropertyMetadata(null));

    public BindingProxy() { }

    public BindingProxy(object data)
    {
        this.Data = data;
    }

    public object Data
    {
        get => this.GetValue(DataProperty);
        set => this.SetValue(DataProperty, value);
    }

    protected override Freezable CreateInstanceCore() => new BindingProxy();
}
