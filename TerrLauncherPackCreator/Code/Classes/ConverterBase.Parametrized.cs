using System;
using System.Globalization;
using System.Windows.Data;
using MVVM_Tools.Code.Utils;

namespace MVVM_Tools.Code.Classes
{
    /// <summary>
    /// Base class for handling converting values
    /// </summary>
    /// <typeparam name="TSource">Source value tape</typeparam>
    /// <typeparam name="TParameter">Parameter value type</typeparam>
    /// <typeparam name="TTarget">Target value property</typeparam>
    public abstract class ConverterBase<TSource, TParameter, TTarget> : IValueConverter
    {
        private static readonly bool IsSourceNullable = default(TSource) == null;
        private static readonly bool IsParameterNullable = default(TParameter) == null;
        private static readonly bool IsTargetNullable = default(TTarget) == null;

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TSource typedSource = value == null ? GetSourceIfNull() : CommonUtils.CheckValueTypeAndCast<TSource>(value);
            TParameter typedParameter = parameter == null ? GetParameterIfNull() : CommonUtils.CheckValueTypeAndCast<TParameter>(parameter);

            return ConvertInternal(typedSource, typedParameter, culture);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TTarget typedTarget = value == null ? GetTargetIfNull() : CommonUtils.CheckValueTypeAndCast<TTarget>(value);
            TParameter typedParameter = parameter == null ? GetParameterIfNull() : CommonUtils.CheckValueTypeAndCast<TParameter>(parameter);

            return ConvertBackInternal(typedTarget, typedParameter, culture);
        }

        /// <summary>
        /// Converts provided value to the target type
        /// </summary>
        /// <param name="value">Source value</param>
        /// <param name="parameter">Parameter value</param>
        /// <param name="culture">Converting culture</param>
        /// <returns>Converted value</returns>
        public abstract TTarget ConvertInternal(TSource value, TParameter parameter, CultureInfo culture);

        /// <summary>
        /// Converts provided value to the source type
        /// </summary>
        /// <param name="value">Target value</param>
        /// <param name="parameter">Parameter value</param>
        /// <param name="culture">Converting culture</param>
        /// <returns>Converted value</returns>
        public virtual TSource ConvertBackInternal(TTarget value, TParameter parameter, CultureInfo culture)
        {
            throw new InvalidOperationException($"'{GetType().FullName}' converter can't handle back conversation");
        }

        /// <summary>
        /// Returns source value linked to the <code>null</code> variant
        /// </summary>
        protected virtual TSource GetSourceIfNull()
        {
            if (IsSourceNullable)
                return default;

            throw new InvalidOperationException("Operation is not supported on structs");
        }

        /// <summary>
        /// Returns parameter value linked to the <code>null</code> variant
        /// </summary>
        protected virtual TParameter GetParameterIfNull()
        {
            if (IsParameterNullable)
                return default;

            throw new InvalidOperationException("Operation is not supported on structs");
        }

        /// <summary>
        /// Returns target value linked to the <code>null</code> variant
        /// </summary>
        protected virtual TTarget GetTargetIfNull()
        {
            if (IsTargetNullable)
                return default;

            throw new InvalidOperationException("Operation is not supported on structs");
        }
    }
}
