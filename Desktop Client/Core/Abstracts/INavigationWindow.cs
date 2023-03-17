using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

public interface INavigationWindow
{
    Task Display();

    Task Hide();
}