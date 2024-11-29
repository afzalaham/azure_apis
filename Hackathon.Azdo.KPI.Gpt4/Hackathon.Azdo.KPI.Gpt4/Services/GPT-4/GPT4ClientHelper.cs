using Azure;
using Azure.AI.OpenAI;
using Hackathon.Azdo.KPI.Analyzer.API.Services.GPT_4.Interface;

namespace Hackathon.Azdo.KPI.Analyzer.API.Services.GPT_4
{
    public class GPT4ClientHelper : IGPT4ClientHelper
    {
        private readonly OpenAIClient _client;

        public GPT4ClientHelper()
        {
            _client = new OpenAIClient(new Uri("https://alegeus-hackathon2024.openai.azure.com"), new AzureKeyCredential("9613f4e0352c4eaeb9b37d05136a19a8"));
        }

        public async Task<ChatCompletions> GetCompletionsAsync(string engineName, ChatRequestUserMessage chatMessage)
        {
            var ccOptions = new ChatCompletionsOptions
            {
                DeploymentName = engineName,
                Temperature = 0.7f,
                MaxTokens = 800,
                NucleusSamplingFactor = 0.5f,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
                ChoiceCount = 1
            };
            try
            {
                var abc = await _client.GetChatCompletionsAsync(ccOptions);
                return await _client.GetChatCompletionsAsync(ccOptions);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> SummarizeJsonAsync(string engineName, string jsonInput)
        {

            // Configure chat options
            var chatOptions = new ChatCompletionsOptions
            {
                DeploymentName = engineName,
                Temperature = 0.7f,
                MaxTokens = 800,
                NucleusSamplingFactor = 0.5f,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
                ChoiceCount = 1
            };

            chatOptions.Messages.Add(new ChatRequestUserMessage($"Here is the data: {jsonInput}. Summarize the key points."));

            try
            {
                // Send the request to Azure OpenAI
                var response = await _client.GetChatCompletionsAsync(chatOptions);
                Console.WriteLine(response);
                Console.WriteLine(response.Value.Choices[0].Message.Content.Trim());
                // Extract and return the content from the first choice
                return response.Value.Choices[0].Message.Content.Trim();
                
            }
            catch (Exception ex)
            {
                // Log or handle exception as needed
                throw new InvalidOperationException("Error while summarizing JSON input.", ex);
            }
        }
    }
}
