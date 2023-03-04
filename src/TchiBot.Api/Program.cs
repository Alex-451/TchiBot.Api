using TchiBot.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/tarifstatus", async () =>
{
    var utils = new TchiBotUtils();
    var clientSessionId = utils.GetClientSessionId();

    var publicKey = await utils.GetPublicKeyAsync(clientSessionId);

    var encryptedUsername = utils.GetEncryptedText(builder.Configuration["Tchibo:Username"], publicKey);
    var encryptedPassword = utils.GetEncryptedText(builder.Configuration["Tchibo:Password"], publicKey);

    var securityToken = await utils.GetSecurityTokenAsync(encryptedUsername, encryptedPassword, clientSessionId);

    return await utils.GetTariffStatusList(securityToken, clientSessionId);
})
.WithName("GetTarifStatus")
.WithOpenApi(); ;

app.Run();
