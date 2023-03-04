using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Utilities.IO.Pem;
using System.Text;
using System.Text.Json;
using TchiBot.Api.Models;
using Org.BouncyCastle.Security;

namespace TchiBot.Api;

public class TchiBotUtils
{
    public string GetClientSessionId()
    {
        var rand = new Random();
        var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        string sessionId = "tm-";

        for (int i = 3; i < 39; i++)
        {
            if (i == 11 || i == 16 || i == 21 || i == 26)
            {
                sessionId += "-";
            }
            else
            {
                sessionId += chars[rand.Next(chars.Length)];
            }
        }

        return sessionId;
    }

    public async Task<string> GetPublicKeyAsync(string clientSessionId)
    {
        using HttpClient client = new();

        var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new("action", "getPublicKey"),
            new("clientSessionID", clientSessionId)
        });

        var response = await client.PostAsync("https://public-service.tchibo-mobil.de/loginservice/jsp/service.jsp", content);
        response.EnsureSuccessStatusCode();

        var publicKey = await JsonSerializer.DeserializeAsync<PublicKeyDto>(await response.Content.ReadAsStreamAsync());

        return "-----BEGIN PUBLIC KEY-----\n" + publicKey.PublicKey + "\n-----END PUBLIC KEY-----";
    }

    public string GetEncryptedText(string textToEncrypt, string pubKey)
    {

        var bytesToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt);

        var encryptEngine = new Pkcs1Encoding(new RsaEngine());

        using (var txtreader = new StringReader(pubKey))
        {

            var pemReader = new PemReader(txtreader);
            var keyParameter = PublicKeyFactory.CreateKey(pemReader.ReadPemObject().Content);
            encryptEngine.Init(true, keyParameter);
        }

        var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
        var ree = Convert.ToHexString(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
        return ree;
    }

    public async Task<string> GetSecurityTokenAsync(string encryptedUsername, string encryptedPassword, string clientSessionId)
    {
        using HttpClient client = new();

        var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new("action", "submitLoginFormLoginByPassword"),
            new("clientSessionID", clientSessionId),
            new("username", encryptedUsername),
            new("password", encryptedPassword)
        });

        var response = await client.PostAsync("https://public-service.tchibo-mobil.de/loginservice/jsp/service.jsp", content);
        response.EnsureSuccessStatusCode();

        var securityToken = await JsonSerializer.DeserializeAsync<SecurityTokenDto>(await response.Content.ReadAsStreamAsync());

        return securityToken.SecurityToken;
    }

    public async Task<TariffStatusBeanDto> GetTariffStatusList(string securityToken, string clientSessionId)
    {
        
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://tchibo-mobil.de/cfapi/ws/rest/customerfrontendapi/getTariffStatusList");
        var content = new StringContent($"-----------------------------403935067436909090372268398137  \r\nContent-Disposition: form-data; name=\"serviceInfo\"; filename=\"serviceInfo\"\r\nContent-Type: application/json\r\n\r\n{{\r\n    \"sessionIDClient\": \"{clientSessionId}\",  \r\n    \"securityTokenClient\": \"{securityToken}\",\r\n    \"targetMandatID\": 1,\r\n    \"requestStartTime\": 1658420506888,\r\n    \"lastRequestTime\": 1658420506888,  \r\n    \"callPath\": \"https://tchiboweb.tchibo-mobil.de/dashboard\",\r\n    \"callReference\": \"TariffService\"\r\n}}\r\n-----------------------------403935067436909090372268398137--");
        request.Content = content;

        request.Content.Headers.Remove("Content-Type");
        request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=---------------------------403935067436909090372268398137");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var tariffStatus = await JsonSerializer.DeserializeAsync<TariffStatusDto>(await response.Content.ReadAsStreamAsync());
        return tariffStatus.TariffStatusBeans[0];
       
    }
}
