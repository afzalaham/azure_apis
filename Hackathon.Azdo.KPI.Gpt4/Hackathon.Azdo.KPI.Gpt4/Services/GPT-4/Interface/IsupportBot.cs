using Microsoft.Bot.Builder;

namespace Hackathon.Azdo.KPI.Analyzer.API.Services.GPT_4.Interface
{
    public interface IsupportBot
    {


        // Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken);
        bool IsJson(string input);
        Task OnMessageActivityAsync(TurnContext turnContext, CancellationToken cancellationToken);
    }
}
