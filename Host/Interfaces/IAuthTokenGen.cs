namespace Host.Interfaces;

public interface IAuthTokenGen
{
    string GetToken(string username, string password);
}