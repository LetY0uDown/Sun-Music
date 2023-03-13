using Host.Interfaces;
using Host.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHashIDGenerator, HashIDGenerator>();
builder.Services.AddSingleton<IPasswordEncoder, PasswordEncoder>();
builder.Services.AddSingleton<IAuthTokenGen, JWTTokenGenerator>();

builder.Services.AddDbContext<DatabaseContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();