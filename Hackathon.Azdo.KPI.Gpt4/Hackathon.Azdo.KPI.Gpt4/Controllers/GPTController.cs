using Hackathon.Azdo.KPI.Analyzer.API.Services.GPT_4.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Schema;


namespace Hackathon.Azdo.KPI.Gpt4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GPTController : ControllerBase
    {



        private readonly IsupportBot _echoBot;


        public GPTController(IsupportBot echoBot)
        {

            _echoBot = echoBot;

        }


        [HttpPost("test")]
        public async Task<IActionResult> TestBot([FromBody] Activity message)
        {
            if (message == null)
            {
                return BadRequest("Activity cannot be null.");
            }

            // Create a TurnContext for processing
            var adapter = new TestAdapter();
            var turnContext = new TurnContext(adapter, message);
            var cancellationToken = CancellationToken.None;

            // Process the activity with your bot logic
            await _echoBot.OnMessageActivityAsync(turnContext, cancellationToken);

            // Retrieve the bot's response (assuming a single outgoing message for simplicity)
            return Ok("output in console");
        }



        //[HttpPost("BOT")]
        //public async Task<IActionResult> TestBot([FromBody] object request)
        //{
        //    if (request == null)
        //    {
        //        return BadRequest("Request cannot be null.");
        //    }

        //    Activity activity;
        //    try
        //    {
        //        // Dynamically deserialize the object into an Activity
        //        var json = JsonConvert.SerializeObject(request);
        //        activity = JsonConvert.DeserializeObject<Activity>(json);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Failed to parse message: {ex.Message}");
        //    }

        //    if (activity == null)
        //    {
        //        return BadRequest("Deserialized activity is null.");
        //    }

        //    // Create a TurnContext for processing
        //    var adapter = new TestAdapter();
        //    var turnContext = new TurnContext(adapter, activity);
        //    var cancellationToken = CancellationToken.None;

        //    // Process the activity with your bot logic
        //    try
        //    {
        //        await _echoBot.OnMessageActivityAsync(turnContext, cancellationToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Bot processing failed: {ex.Message}");
        //    }

        //    // Retrieve bot responses
        //    var responses = adapter.ActiveQueue
        //        .Where(activity => activity.Type == ActivityTypes.Message)
        //        .Select(activity => ((IMessageActivity)activity).Text)
        //        .Where(text => !string.IsNullOrEmpty(text))
        //        .ToList();

        //    return Ok(new { BotReplies = responses });
        //}

    }
}