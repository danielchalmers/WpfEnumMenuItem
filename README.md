# WpfEnumMenuItem

A drop-in replacement for MenuItem that binds to an enum.

[![NuGet](https://img.shields.io/nuget/v/WpfEnumMenuItem.svg)](https://www.nuget.org/packages/WpfEnumMenuItem)

```csharp
// Declare the enum.
public enum MyEnum {
    One,
    Two,
    [Description("Third item!")]
    Three,
    Four,
}

// The property we'll bind to.
public MyEnum MyEnum { get; set; }
```

```xaml
<Menu DataContext="...">
    <emi:EnumMenuItem
        Header="MyEnum"
        Binding="{Binding MyEnum}" />
</Menu>
```

![Screenshot](https://user-images.githubusercontent.com/7112040/136148767-0bdba060-55f7-4653-8d75-ff9ddbde7c01.png)
