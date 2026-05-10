using System.ClientModel;
using OpenAI;
using OpenAI.Chat;
using Serilog;

namespace CattyBot.services;


public class OpenRouterBot
{
    private static readonly string[] ModelsPipeline = 
    [
        "tencent/hy3-preview:free",
        "deepseek/deepseek-v4-flash",
        "google/gemini-2.5-flash-lite"
    ];

    private readonly List<ChatClient> _clients;

    public OpenRouterBot()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_TOKEN") 
                     ?? throw new Exception("Переменная OPENAI_TOKEN отсутствует");
        
        var endpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT") 
                       ?? "https://openrouter.ai/api/v1";

        var credential = new ApiKeyCredential(apiKey);
        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(endpoint)
        };

        _clients = [];
        foreach (var model in ModelsPipeline)
        {
            _clients.Add(new ChatClient(model, credential, options));
        }
    }

    public async Task<string> GenerateTextResponse(List<ChatMessage> messages, CancellationToken cancelToken)
    {
        var exceptions = new List<Exception>();

        foreach (var client in _clients)
        {
            try
            {
                ChatCompletion completion = await client.CompleteChatAsync(messages, cancellationToken: cancelToken);
                return completion.Content[0].Text;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        throw new AggregateException("No response from any models :(", exceptions);
    }
}
