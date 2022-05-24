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
        ItemsSource = Choices;

        ItemContainerStyle = new()
        {
            TargetType = typeof(MenuItem),
            BasedOn = FindResource(typeof(MenuItem)) as Style,
            Setters =
            {
                new Setter(HeaderProperty, new Binding(nameof(EnumMenuItemChoiceWrapper.DisplayName))),
                new Setter(IsCheckableProperty, true),
                new Setter(IsCheckedProperty, new Binding(nameof(EnumMenuItemChoiceWrapper.IsChecked))),
            }
        };
    }

    public Enum Binding
    {
        get => (Enum)GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    protected ObservableCollection<EnumMenuItemChoiceWrapper> Choices { get; } = new();

    private static void BindingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not Enum)
            throw new InvalidOperationException("Tried to bind to a non-enum type.");
        if (o is not EnumMenuItem menuItem)
            throw new InvalidOperationException($"Dependency object type must be {nameof(EnumMenuItem)}.");

        var isSameType = e.OldValue?.GetType() == e.NewValue?.GetType();
        if (isSameType)
        {
            // Update checkboxes to reflect changed binding.
            foreach (var choice in menuItem.Choices)
                choice.UpdateIsChecked();
        }
        else
        {
            // Repopulate choices for new enum.
            menuItem.Choices.Clear();
            foreach (var value in Enum.GetValues(menuItem.Binding.GetType()))
                menuItem.Choices.Add(new(menuItem, (Enum)value));
        }
    }

    protected class EnumMenuItemChoiceWrapper : INotifyPropertyChanged
    {
        public EnumMenuItemChoiceWrapper(EnumMenuItem enumMenuItem, Enum @enum)
        {
            EnumMenuItem = enumMenuItem;
            Enum = @enum;
            Name = @enum.ToString();
            DisplayName = GetAttributeOrDefault<DescriptionAttribute>(@enum)?.Description ?? Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private EnumMenuItem EnumMenuItem { get; }
        public Enum Enum { get; }
        public string Name { get; }
        public string DisplayName { get; }

        public bool IsChecked
        {
            get => EnumMenuItem.Binding.Equals(Enum);
            set
            {
                if (value)
                    EnumMenuItem.Binding = Enum;
            }
        }

        internal void UpdateIsChecked() =>
            PropertyChanged?.Invoke(this, new(nameof(IsChecked)));

        private static TAttribute GetAttributeOrDefault<TAttribute>(Enum @enum) where TAttribute : Attribute =>
            @enum?.GetType()?.GetMember(@enum.ToString())?.FirstOrDefault()?.GetCustomAttributes(typeof(TAttribute), false)?.FirstOrDefault() as TAttribute;
    }
}