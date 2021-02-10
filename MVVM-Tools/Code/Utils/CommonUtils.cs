using System;

namespace MVVM_Tools.Code.Utils
{
    /// <summary>
    /// Provides common functions
    /// </summary>
    public static class CommonUtils
    {
        /// <summary>
        /// Casts value to the target type throwing errors on incorrect values
        /// </summary>
        /// <param name="value">Value to cast</param>
        /// <typeparam name="TValidType">Target value type</typeparam>
        /// <exception cref="ArgumentException">Is thrown then value can not be cast</exception>
        public static TValidType CheckValueTypeAndCast<TValidType>(object value)
        {
            if (ReferenceEquals(value, null))
            {
                var defaultValue = default(TValidType);

                if (defaultValue == null)
                    return default;

                throw new ArgumentException($"Provided value is null while `{typeof(TValidType).FullName}` is not nullable");
            }

            if (value is TValidType)
                return (TValidType)value;

            throw new ArgumentException(
                "Invalid parameter type\n" +
                $"Expected: `{typeof(TValidType).FullName}`\n" +
                $"Actual: `{value.GetType().FullName}`"
            );
        }
    }
}
