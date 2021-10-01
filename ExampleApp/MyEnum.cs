using System.ComponentModel;

namespace ExampleApp
{
    public enum MyEnum
    {
        Item1,
        
        Item2,

        [Description("Item with a description")]
        ItemWithDescription,

        LastItem,
    }
}