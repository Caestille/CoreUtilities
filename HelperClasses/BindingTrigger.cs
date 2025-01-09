namespace CoreUtilities.HelperClasses
{
    using System.Windows.Data;
    using System.Windows;
    using System.ComponentModel;

    public class BindingTrigger : INotifyPropertyChanged
    {
        public BindingTrigger()
            => this.Binding = new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(this.Value))
            };

        public event PropertyChangedEventHandler? PropertyChanged;

        public Binding Binding { get; }

        public void Refresh()
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Value)));

        public object? Value { get; }
    }
}
