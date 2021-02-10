namespace CrossPlatform.Code.Interfaces
{
    public interface IZipHelper
    {
        void Extract(string inputZipPath, string targetDirectory);
    }
}