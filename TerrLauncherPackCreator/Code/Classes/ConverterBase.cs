namespace MVVM_Tools.Code.Classes
{
    /// <summary>
    /// Base class for handling converting values with a parameter of the <see cref="object"/> type
    /// </summary>
    /// <typeparam name="TSource">Source value tape</typeparam>
    /// <typeparam name="TTarget">Target value property</typeparam>
    public abstract class ConverterBase<TSource, TTarget> : ConverterBase<TSource, object, TTarget>
    {
    }
}
