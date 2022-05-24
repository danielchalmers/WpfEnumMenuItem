using System.ComponentModel;

namespace ExampleApp;

public enum MyEnum
{
    One,

    Two,

    [Description("Third item!")]
    Three,

    Four,
}