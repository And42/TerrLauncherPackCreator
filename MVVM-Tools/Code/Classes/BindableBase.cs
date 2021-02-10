using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM_Tools.Code.Classes
{
    /// <summary>
    /// Base class for property changes notifications
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Sets the value of a specified property if it is different from the current one (checking using <see cref="EqualityComparer{T}.Default"/>)
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="storage">FieldProperty's backing field</param>
        /// <param name="value">New value</param>
        /// <param name="propertyName">FieldProperty name</param>
        /// <returns><code>True</code> if <see cref="storage"/> value is changed; otherwise, <code>False</code></returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Sets the value of a specified property if it is different from the current one (checking using <see cref="object.ReferenceEquals"/>)
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="storage">FieldProperty's backing field</param>
        /// <param name="value">New value</param>
        /// <param name="propertyName">FieldProperty name</param>
        /// <returns><code>True</code> if <see cref="storage"/> value is changed; otherwise, <code>False</code></returns>
        protected bool SetPropertyRef<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) where T : class
        {
            if (ReferenceEquals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Occurs on property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Used to call <see cref="PropertyChanged"/> event on a specified property
        /// </summary>
        /// <param name="propertyName">FieldProperty to use while raising <see cref="PropertyChanged"/> event</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
