using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MVVM_Tools.Code.Classes;

namespace MVVM_Tools.Code.Providers
{
    /// <summary>
    /// Provider for the properties (use <see cref="BindableBase.PropertyChanged"/> to subscribe for changes).
    /// Uses <see cref="BindableBase.SetProperty{TPropertyType}"/> when setting new value
    /// </summary>
    /// <typeparam name="TPropertyType">Property type</typeparam>
    public class FieldProperty<TPropertyType> : IProperty<TPropertyType>
    {
        private TPropertyType _value;
        private bool _isReadonly;
 
        /// <summary>
        /// Current value of the provider
        /// </summary>
        public TPropertyType Value
        {
            get => _value;
            set
            {
                if (_isReadonly)
                    throw new NotSupportedException("Property is readonly");

                if (EqualityComparer<TPropertyType>.Default.Equals(_value, value))
                    return;

                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of the <see cref="T:MVVM_Tools.Code.Providers.FieldProperty`1" /> class
        /// </summary>
        public FieldProperty(): this(default) { }

        /// <summary>
        /// Creates a new instance of the <see cref="FieldProperty{TPropertyType}"/> class
        /// </summary>
        /// <param name="initialValue">Initial value for the backing field</param>
        public FieldProperty(TPropertyType initialValue)
        {
            _value = initialValue;
        }

        public IReadonlyProperty<TPropertyType> AsReadonly()
        {
            _isReadonly = true;
            return this;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
