using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfEnumMenuItem;

public class EnumMenuItem : MenuItem
{
    public static readonly DependencyProperty BindingProperty =
        DependencyProperty.Register(
            nameof(Binding),
            typeof(Enum),
            typeof(EnumMenuItem),
            new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, BindingChanged)
        );

    public EnumMenuItem()
    {
        ItemsSource = WrappedItems;

        ItemContainerStyle = new()
        {
            TargetType = typeof(MenuItem),
            BasedOn = FindResource(typeof(MenuItem)) as Style,
            Setters =
            {
                new Setter(HeaderProperty, new Binding(nameof(EnumMenuItemWrapper.Header))),
                new Setter(IsCheckableProperty, true),
                new Setter(IsCheckedProperty, new Binding(nameof(EnumMenuItemWrapper.IsChecked))),
            }
        };
    }

    public Enum Binding
    {
        get => (Enum)GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    protected ObservableCollection<EnumMenuItemWrapper> WrappedItems { get; } = new();

    private static void BindingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o is not EnumMenuItem menuItem)
            throw new InvalidOperationException($"Dependency object must be of type {nameof(EnumMenuItem)}.");
        if (e.NewValue is not Enum @enum)
            throw new InvalidOperationException("Tried to bind to a non-enum type.");

        var isSameType = e.OldValue?.GetType() == @enum.GetType();
        if (isSameType)
        {
            // Update checked status for all items.
            foreach (var choice in menuItem.WrappedItems)
                choice.UpdateIsChecked();
        }
        else
        {
            // Populate for new type.
            menuItem.WrappedItems.Clear();
            foreach (var value in Enum.GetValues(menuItem.Binding.GetType()))
                menuItem.WrappedItems.Add(new(menuItem, (Enum)value));
        }
    }

    protected class EnumMenuItemWrapper : INotifyPropertyChanged
    {
        private readonly EnumMenuItem _parent;

        public EnumMenuItemWrapper(EnumMenuItem parent, Enum @enum)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Enum = @enum ?? throw new ArgumentNullException(nameof(@enum));
            Header = GetAttributeOrDefault<DescriptionAttribute>(@enum)?.Description ?? @enum.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Enum Enum { get; }
        public string Header { get; }

        public bool IsChecked
        {
            get => _parent.Binding.Equals(Enum);
            set
            {
                if (value)
                    _parent.Binding = Enum;
            }
        }

        internal void UpdateIsChecked() =>
            PropertyChanged?.Invoke(this, new(nameof(IsChecked)));

        private static T GetAttributeOrDefault<T>(Enum @enum) where T : Attribute =>
            @enum.GetType().GetMember(@enum.ToString())?.FirstOrDefault()?.GetCustomAttributes(typeof(T), false)?.FirstOrDefault() as T;
    }
}