# WpfEnumMenuItem
A drop-in replacement for MenuItem that binds to an enum

[![NuGet](https://img.shields.io/nuget/v/WpfEnumMenuItem.svg)](https://www.nuget.org/packages/WpfEnumMenuItem)

```csharp
// Declare the enum.
public enum MyEnum {
    Item1, Item2, [Description("Item with a description")] ItemWithDescription, LastItem
}

// The property we'll bind to.
public MyEnum MyEnum { get; set; } // TODO: Notify PropertyChanged.
```

```xaml
<Menu DataContext="...">
    <emi:EnumMenuItem Header="EnumMenuItem bound to MyEnum" Binding="{Binding MyEnum}" />
</Menu>
```

![image](https://user-images.githubusercontent.com/7112040/135589479-de11ff73-f85e-4cbc-bb63-38ef78be4a28.png)
