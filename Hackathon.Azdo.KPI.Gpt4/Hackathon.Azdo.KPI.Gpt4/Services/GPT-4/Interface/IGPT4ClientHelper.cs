using Azure.AI.OpenAI;

namespace Hackathon.Azdo.KPI.Analyzer.API.Services.GPT_4.Interface
{
    public interface IGPT4ClientHelper
    {
        Task<ChatCompletions> GetCompletionsAsync(string engineName, ChatRequestUserMessage chatMessage);

        Task<string> SummarizeJsonAsync(string engineName, string jsonInput);
    }
}
