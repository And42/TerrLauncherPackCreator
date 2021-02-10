namespace MVVM_Tools.Code.Providers
{
    public interface IProperty<TPropertyType> : IReadonlyProperty<TPropertyType>
    {
        new TPropertyType Value { get; set; }
    }
}