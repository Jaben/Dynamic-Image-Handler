namespace DynamicImageHandler.Utils
{
    public interface ISymCryptKey
    {
        byte[] Key { get; }

        byte[] Salt { get; }
    }
}