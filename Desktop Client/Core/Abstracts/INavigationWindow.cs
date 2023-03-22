using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

public interface INavigationWindow : IView
{
    Task Display();

    Task Hide();
}