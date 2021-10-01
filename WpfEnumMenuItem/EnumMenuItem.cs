using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfEnumMenuItem
{
    public class EnumMenuItem : MenuItem
    {
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register(
                nameof(Binding),
                typeof(Enum),
                typeof(EnumMenuItem),
                new PropertyMetadata(EnumChanged)
            );

        public EnumMenuItem()
        {
            ItemsSource = Choices;

            ItemContainerStyle = new Style
            {
                TargetType = typeof(MenuItem),
                BasedOn = (Style)FindResource(typeof(MenuItem)),
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

        private static void EnumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not Enum)
                throw new InvalidOperationException($"Can't bind to a non-enum type in an {nameof(EnumMenuItem)}.");
            if (o is not EnumMenuItem menuItem)
                throw new InvalidOperationException($"Dependency object must be a {nameof(EnumMenuItem)}.");

            var isSameType = e.OldValue?.GetType() == e.NewValue?.GetType();

            if (isSameType) // Update checkboxes to reflect changed binding.
            {
                foreach (var choice in menuItem.Choices)
                    choice.UpdateIsChecked();
            }
            else // Populate choices for new enum.
            {
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
                DisplayName = GetAttribute<DescriptionAttribute>(@enum)?.Description ?? Name;
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
                    if (value == true)
                        EnumMenuItem.Binding = Enum;
                }
            }

            internal void UpdateIsChecked() =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));

            private static T GetAttribute<T>(Enum enumValue) where T : Attribute
            {
                var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
                return (T)memberInfo?.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            }
        }
    }
}