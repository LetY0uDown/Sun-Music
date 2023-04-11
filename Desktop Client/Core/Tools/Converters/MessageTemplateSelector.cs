using Models.Database;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_Client.Core.Tools.Converters;

internal sealed class MessageTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        FrameworkElement page = container as FrameworkElement;
        var msg = item as Message;

        return msg.Sender.ID switch
        {
            "00000000-0000-0000-0000-000000000000" => page.FindResource("ServerMessageTemplate") as DataTemplate,
            _ => page.FindResource("UserMessageTemplate") as DataTemplate,
        };
    }
}