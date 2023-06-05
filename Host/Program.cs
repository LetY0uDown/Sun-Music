using Host.Services;
using Host.Services.Database;
using Host.Tools.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SetupBase();
builder.Services.AddServices();
builder.Services.AddSignalR();
builder.Services.AddScoped<DatabaseContext, MySqlContext>();
builder.Services.AddRepositories();
builder.Services.AddAuthorization(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MainHub>("MainHub");

app.MapControllers();

app.Run();