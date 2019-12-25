namespace Crypto.RSA.Encoders
{
    public interface IEncoder
    {
        string GetBytes(string str);
        string GetString(string bytes);
    }
}
