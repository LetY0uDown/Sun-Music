using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Models.Database;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Desktop_Client.Core.ViewModels.Messanger;

[Lifetime(Lifetime.Transient)]
public sealed class ChatCreatingViewModel : ViewModel
{
    private readonly IAPIClient _client;
    private readonly IConfiguration _config;

    public ChatCreatingViewModel(IAPIClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public UICommand SelectImageCommand { get; private set; }

    public UICommand CreateChatCommand { get; private set; }

    public string Title { get; set; }

    public BitmapImage Image { get; set; }

    public override Task Display()
    {
        CreateChatCommand = new(async o => {
            Chat chat = new() {
                ID = "id",
                Title = Title,
                ImageBytes = ImageConverter.BytesFromImage(Image),
                CreatorID = App.AuthorizeData.ID,
                Creator = await _client.GetAsync<User>($"Users/{App.AuthorizeData.ID}")
            };

            await _client.PostAsync<Chat, Chat>(chat, "Chats/Create");

        }, b => !string.IsNullOrWhiteSpace(Title) &&
                Image is not null);

        SelectImageCommand = new(o => {

            var fileDialog = new OpenFileDialog {
                Filter = _config["Filters:Images"]
            };

            if (fileDialog.ShowDialog() == true) {
                Image = ImageConverter.CreateImageFromFile(fileDialog.FileName);
            }
        });

        return base.Display();
    }
}