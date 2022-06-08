# WpfEnumMenuItem [![NuGet](https://img.shields.io/nuget/v/WpfEnumMenuItem.svg)](https://www.nuget.org/packages/WpfEnumMenuItem)

A drop-in replacement for [`MenuItem`](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.menuitem) that binds to an enum.

```csharp
// Declare the enum.
public enum MyEnumType
{
    One,

    Two,

    // Automatically reads the attribute.
    [Description("Third item!")]
    Three,

    Four,
}

// The property we'll bind to.
public MyEnumType MyEnumProperty { get; set; }
```

```xaml
<!-- xmlns:emi="clr-namespace:WpfEnumMenuItem;assembly=WpfEnumMenuItem" -->

<Menu DataContext="...">
    <emi:EnumMenuItem Header="MyEnum" Binding="{Binding MyEnumProperty}" />
</Menu>
```

![Screenshot](https://user-images.githubusercontent.com/7112040/136148767-0bdba060-55f7-4653-8d75-ff9ddbde7c01.png)

Build and run the ExampleApp for a working demo.
