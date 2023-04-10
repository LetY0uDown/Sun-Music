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

    public ChatsController (IHubContext<MainHub> hub, DatabaseContext context, IIDGenerator idGen)
    {
        _hub = hub;
        _context = context;
        _idGen = idGen;
    }

    [HttpPost("Create")]
    public async Task<ActionResult<Chat>> CreateChat ([FromBody] Chat chat)
    {
        try {
            var isUnique = _context.Chats.Any(c => c.Title == chat.Title);

            if (!isUnique) {
                return BadRequest();
            }

            chat.ID = _idGen.GenerateID();

            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();

            await _hub.Clients.Group("Chats").SendAsync("RecieveChat", chat);

            return Ok(chat);

        }
        catch (Exception ex) {
            return Problem();
        }
    }

    [HttpGet("User/{id}")]
    public async Task<ActionResult<IEnumerable<Chat>>> GetChatsByUser ([FromRoute] string id)
    {
        try {
            var chatIDs = await _context.ChatMembers.Where(e => e.UserId == id).Select(e => e.ChatId).ToListAsync();

            List<Chat> result = new List<Chat>();

            chatIDs.ForEach(async id => {
                result.Add(await _context.Chats.FindAsync(id));
            });

            return Ok(result);

        }
        catch (Exception ex) {
            return Problem();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Chat>> GetChatByID ([FromRoute] string id)
    {
        try {
            var chat = await _context.Chats.FindAsync(id);

            if (chat is null) {
                return NotFound();
            }

            return Ok(chat);
        }
        catch (Exception ex) {
            return Problem();
        }
    }

    [HttpGet("{chatID}/Join"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> JoinChat ([FromBody] User user, [FromRoute] string chatID)
    {
        try {
            var cm = await _context.ChatMembers.FirstOrDefaultAsync(e => e.UserId == user.ID && e.ChatId == chatID);

            if (cm is not null) {
                return BadRequest();
            }

            cm = new() {
                ID = _idGen.GenerateID(),
                ChatId = chatID,
                UserId = user.ID
            };

            await _context.ChatMembers.AddAsync(cm);
            await _context.SaveChangesAsync();

            var chat = await _context.Chats.FindAsync(chatID);

            await _hub.Clients.Group($"Chat - {chat.Title}").SendAsync("RecieveMember", user);

            return NoContent();

        }
        catch (Exception ex) {
            return Problem();
        }
    }

    [HttpGet("{chatID}/Messages")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessagesFromChat ([FromRoute] string chatID)
    {
        try {
            return await _context.Messages.Include(msg => msg.Sender).Where(msg => msg.ChatID == chatID).ToListAsync();
        }
        catch (Exception ex) {
            return Problem();
        }
    }

    [HttpPost("{chatID}/Send/{senderID}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> SendMessageToChat ([FromBody] Message message, [FromRoute] string chatID, [FromRoute] string senderID)
    {
        try {
            var chat = await _context.Chats.FindAsync(chatID);

            message.ID = _idGen.GenerateID();

            message.Sender = await _context.Users.FindAsync(senderID);

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            await _hub.Clients.Group($"Chat - {chat.Title}").SendAsync("RecieveMessage", message);

            return NoContent();
        }
        catch (Exception ex) {
            return Problem();
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

            foreach (var u in users) {
                publicUsers.Add(u);
            }

            return Ok(publicUsers);
        }
        catch (Exception ex) {
            return Problem();
        }
    }
}