using Newtonsoft.Json;
using System.Text;

namespace ChatgptSdk.Lib;

sealed class ChatGptSdk : IChatGptSdk
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://api.openai.com/v1";

    public ChatGptSdk(AppConfig appConfig)
    {
        _apiKey = appConfig.ApiKey;
        _model = appConfig.Model;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<string> SendMessageAsync(string message)
    {
        var requestPayload = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = message }
            }
        };

        var jsonContent = JsonConvert.SerializeObject(requestPayload);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{ApiBaseUrl}/chat/completions", content);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Request failed with status code {response.StatusCode}");
        }
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
        ArgumentNullException.ThrowIfNull(jsonResponse);
        
        return jsonResponse.choices[0].message.content.ToString();
    }

    public async Task StreamConversationAsync(string message)
    {
        var requestPayload = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = message }
            },
            stream = true // Enable streaming
        };

        var jsonContent = JsonConvert.SerializeObject(requestPayload);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{ApiBaseUrl}/chat/completions", content);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Request failed with status code {response.StatusCode}");
        }
        var stream = await response.Content.ReadAsStreamAsync();
        using (var reader = new System.IO.StreamReader(stream))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                Console.WriteLine(line); // Print each chunk of the response
            }
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
