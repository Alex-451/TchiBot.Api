using Microsoft.AspNetCore.Hosting;
using TchiBot.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#if DEBUG
app.Urls.Add("http://192.168.178.25:80");
#endif

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/status", async () =>
{
    var utils = new TchiBotUtils();
    var clientSessionId = utils.GetClientSessionId();

    var publicKey = await utils.GetPublicKeyAsync(clientSessionId);

    var encryptedUsername = utils.GetEncryptedText(builder.Configuration["Tchibo:Username"], publicKey);
    var encryptedPassword = utils.GetEncryptedText(builder.Configuration["Tchibo:Password"], publicKey);

    var securityToken = await utils.GetSecurityTokenAsync(encryptedUsername, encryptedPassword, clientSessionId);

    var tarifStatus = await utils.GetTariffStatusList(securityToken, clientSessionId);
    var tarifInfo = await utils.GetTariffInfoSummary(securityToken, clientSessionId);

    return new
    {
        RemainingData = tarifStatus.UsedPercent,
        RemainingDataInMb = tarifStatus.CurrentValue,
        ExtendsOn = tarifInfo.ExtendsOn,
        IsThrottled = tarifStatus.IsThrottled
    };

})
.WithName("GetStatus")
.WithOpenApi();

app.Run();
