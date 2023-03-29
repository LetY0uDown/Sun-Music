using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

public interface INavigationPage : IView
{
    Task Display();

    Task Leave();
}

public interface INavigationWindow : IView
{
    Task Display();

    Task Hide();
}