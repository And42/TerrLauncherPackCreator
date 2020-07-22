using System;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.MarkupExtensions
{
    public class BooleanExtension : MarkupExtension
    {
        [NotNull]
        private static readonly object FalseObject = false;
        [NotNull]
        private static readonly object TrueObject = true;
        
        [NotNull]
        private readonly object _value;

        public BooleanExtension(bool value)
        {
            _value = value ? TrueObject : FalseObject;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => _value;
    }
}