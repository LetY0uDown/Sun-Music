using System.Windows;

namespace Desktop_Client.Core.Tools;

internal static class InfoBox
{
    internal static void Show(string message, string title = "")
    {
        MessageBox.Show(message, title);
    }
}