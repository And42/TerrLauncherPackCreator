using System;
using System.Windows.Markup;

namespace TerrLauncherPackCreator.Code.MarkupExtensions;

public class BooleanExtension : MarkupExtension
{
    private static readonly object FalseObject = false;
    private static readonly object TrueObject = true;
        
    private readonly object _value;

    public BooleanExtension(bool value)
    {
        _value = value ? TrueObject : FalseObject;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => _value;
}