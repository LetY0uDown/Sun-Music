namespace Host.Interfaces;

public interface IPasswordEncoder
{
    string Encode(string pass);
}