using Host.Interfaces;
using Host.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Client;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]/")]
public class ChatsController : ControllerBase
{
    private readonly IHubContext<MainHub> _hub;
    private readonly DatabaseContext _context;
    private readonly IIDGenerator _idGen;
    private readonly ILogger<ChatsController> _logger;

    public ChatsController (IHubContext<MainHub> hub, DatabaseContext context, IIDGenerator idGen, ILogger<ChatsController> logger)
    {
        _hub = hub;
        _context = context;
        _idGen = idGen;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Chat>>> GetAllChats ()
    {
        try {
            return await _context.Chats.Include(chat => chat.Creator)
                                       .ToListAsync();
        }
        catch (Exception e) {
            _logger.LogWarning(e, "Failed to get chats list");
            return Problem("Произошла ошибка. Уточните у администратора");
        }
    }

    [HttpPost("Create"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<Chat>> CreateChat ([FromBody] Chat chat)
    {
        try {
            var isTitleExist = _context.Chats.Any(c => c.Title == chat.Title);

            if (isTitleExist) {
                return BadRequest("Чат с данным названием уже существует. Попробуйте другое название");
            }

            chat.ID = _idGen.GenerateID();

            _context.Users.Attach(chat.Creator);

            var chatMember = new ChatMember() {
                ChatId = chat.ID,
                UserId = chat.Creator.ID,
                ID = _idGen.GenerateID()
            };

            await _context.ChatMembers.AddAsync(chatMember);
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();

            await _hub.Clients.Group("Chats").SendAsync("RecieveChat", chat);

            return Ok(chat);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to create chat");
            return Problem("Произошла ошибка. Уточните у администратора");
        }
    }

    [HttpGet("User/{id}")]
    public async Task<ActionResult<IEnumerable<Chat>>> GetChatsByUser ([FromRoute] string id)
    {
        try {
            var chatIDs = await _context.ChatMembers.Where(e => e.UserId == id)
                                                    .Select(e => e.ChatId)
                                                    .ToListAsync();

            List<Chat> result = _context.Chats.Where(chat => chatIDs.Contains(chat.ID)).ToList();

            return Ok(result);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to get chats by user");
            return Problem("Произошла ошибка. Уточните у администратора");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Chat>> GetChatByID ([FromRoute] string id)
    {
        try {
            var chat = await _context.Chats.Include(chat => chat.Creator)
                                           .FirstOrDefaultAsync(chat => chat.ID == id);

            if (chat is null) {
                return NotFound();
            }

            return Ok(chat);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to get chat by ID");
            return Problem("Произошла ошибка. Уточните у администратора");
        }
    }

    [HttpPost("{chatID}/Leave"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> LeaveChat ([FromBody] User user, [FromRoute] string chatID)
    {
        try {
            var cm = await _context.ChatMembers.FirstOrDefaultAsync(e => e.UserId == user.ID && e.ChatId == chatID);

            if (cm is null) {
                return NotFound("Произошла ошибка. Уточните у администратора");
            }

            _context.ChatMembers.Remove(cm);

            var chat = await _context.Chats.FindAsync(chatID);

            await _hub.Clients.Group($"Chat - {chat.Title}").SendAsync("RemoveMember", (PublicUser)user);

            var emptyUser = await _context.Users.FindAsync(Guid.Empty.ToString());

            var serverMsg = new Message {
                Chat = chat,
                ChatID = chatID,
                ID = _idGen.GenerateID(),
                TimeSended = DateTime.Now,
                SenderID = emptyUser.ID,
                Sender = emptyUser,
                Text = $"Пользователь {user.Username} покинул чат"
            };

            await _hub.Clients.Group($"Chat - {chat.Title}").SendAsync("RecieveMessage", serverMsg);

            await _context.Messages.AddAsync(serverMsg);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to leave chat");
            return Problem("Произошла ошибка. Уточните у администратора");
        }
    }

    [HttpPost("{chatID}/Join"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> JoinChat ([FromBody] User user, [FromRoute] string chatID)
    {
        try {
            var cm = await _context.ChatMembers.FirstOrDefaultAsync(e => e.UserId == user.ID && e.ChatId == chatID);

            if (cm is not null) {
                // It's ok, do not change it!!
                return NoContent();
            }

            cm = new() {
                ID = _idGen.GenerateID(),
                ChatId = chatID,
                UserId = user.ID
            };

            await _context.ChatMembers.AddAsync(cm);

            var chat = await _context.Chats.FindAsync(chatID);

            await _hub.Clients.Group($"Chat - {chat.Title}").SendAsync("RecieveMember", (PublicUser)user);

            var emptyUser = await _context.Users.FindAsync(Guid.Empty.ToString());

            var serverMsg = new Message {
                Chat = chat,
                ChatID = chatID,
                ID = _idGen.GenerateID(),
                TimeSended = DateTime.Now,
                SenderID = emptyUser!.ID,
                Sender = emptyUser,
                Text = $"Пользователь {user.Username} присоединился к чату"
            };

            await _hub.Clients.Group($"Chat - {chat.Title}").SendAsync("RecieveMessage", serverMsg);

            await _context.Messages.AddAsync(serverMsg);
            await _context.SaveChangesAsync();

            return NoContent();

        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to join chat");
            return Problem("Произошла ошибка. Уточните у администратора");
        }
    }

    [HttpGet("{chatID}/Messages")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessagesFromChat ([FromRoute] string chatID)
    {
        try {
            return await _context.Messages.Include(msg => msg.Sender)
                                          .Where(msg => msg.ChatID == chatID)
                                          .OrderBy(chat => chat.TimeSended)
                                          .ToListAsync();
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to get messages from chat");
            return Problem("Произошла ошибка. Уточните у администратора");
        }
    }

    [HttpPost("{chatID}/Send/{senderID}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> SendMessageToChat ([FromBody] Message message, [FromRoute] string chatID, [FromRoute] string senderID)
    {
        try {
            var chat = await _context.Chats.FindAsync(chatID);

            message.ID = _idGen.GenerateID();

            message.Sender = await _context.Users.FindAsync(senderID);
            message.Chat = await _context.Chats.FindAsync(chatID);

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            await _hub.Clients.Group($"Chat - {chat.Title}")
                              .SendAsync("RecieveMessage", message);

            return NoContent();
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to send message");
            return Problem("Произошла ошибка. Уточните у администратора");
        }
    }

    [HttpGet("{chatID}/Members")]
    public async Task<ActionResult<IEnumerable<PublicUser>>> GetMembersByChat ([FromRoute] string chatID)
    {
        try {
            var users = await _context.ChatMembers.Include(e => e.User)
                                                  .Where(e => e.ChatId == chatID)
                                                  .Select(e => e.User)
                                                  .ToListAsync();

            List<PublicUser> publicUsers = new();

            users.ForEach(u =>
                publicUsers.Add(u)
            );

            return Ok(publicUsers);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to get members by chat");
            return Problem("Произошла ошибка. Уточните у администратора");
        }
    }
}