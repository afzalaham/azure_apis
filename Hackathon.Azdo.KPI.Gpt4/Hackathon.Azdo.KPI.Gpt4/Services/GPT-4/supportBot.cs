using Azure.AI.OpenAI;
using Hackathon.Azdo.KPI.Analyzer.API.Services.GPT_4;
using Hackathon.Azdo.KPI.Analyzer.API.Services.GPT_4.Interface;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;

namespace gpt4.Bots
{
    public class supportBot : IsupportBot
    {
        private readonly GPT4ClientHelper _gptClientHelper;

        public supportBot()
        {
            _gptClientHelper = new GPT4ClientHelper();

        }

        public async Task OnMessageActivityAsync(TurnContext turnContext, CancellationToken cancellationToken)
        {
            // Extract user input
            //  string userMessage = turnContext.Activity.Text ?? string.Empty;
            string userMessage = @"
            {
              ""id"": 311351,
              ""rev"": 3,
              ""fields"": {
                ""System.AreaPath"": ""WealthCareNext\\Research"",
                ""System.TeamProject"": ""WealthCareNext"",
                ""System.IterationPath"": ""WealthCareNext\\2024PI4\\Sprint1"",
                ""System.WorkItemType"": ""User Story"",
                ""System.State"": ""New"",
                ""System.Reason"": ""New"",
                ""System.AssignedTo"": {
                  ""displayName"": ""Roman Reyzin"",
                  ""url"": ""https://spsprodcus1.vssps.visualstudio.com/Adbba024b-4985-41b1-86e1-fb7a27d5055f/_apis/Identities/e96b4442-879f-6601-8903-385f7b1c037f"",
                  ""_links"": {
                    ""avatar"": {
                      ""href"": ""https://dev.azure.com/Alegeus-Technologies/_apis/GraphProfile/MemberAvatars/aad.ZTk2YjQ0NDItODc5Zi03NjAxLTg5MDMtMzg1ZjdiMWMwMzdm""
                    }
                  },
                  ""id"": ""e96b4442-879f-6601-8903-385f7b1c037f"",
                  ""uniqueName"": ""rreyzin@alegeus.com"",
                  ""imageUrl"": ""https://dev.azure.com/Alegeus-Technologies/_apis/GraphProfile/MemberAvatars/aad.ZTk2YjQ0NDItODc5Zi03NjAxLTg5MDMtMzg1ZjdiMWMwMzdm"",
                  ""descriptor"": ""aad.ZTk2YjQ0NDItODc5Zi03NjAxLTg5MDMtMzg1ZjdiMWMwMzdm""
                },
                ""System.CreatedDate"": ""2024-11-20T15:01:30.293Z"",
                ""System.CreatedBy"": {
                  ""displayName"": ""Roman Reyzin"",
                  ""url"": ""https://spsprodcus1.vssps.visualstudio.com/Adbba024b-4985-41b1-86e1-fb7a27d5055f/_apis/Identities/e96b4442-879f-6601-8903-385f7b1c037f"",
                  ""_links"": {
                    ""avatar"": {
                      ""href"": ""https://dev.azure.com/Alegeus-Technologies/_apis/GraphProfile/MemberAvatars/aad.ZTk2YjQ0NDItODc5Zi03NjAxLTg5MDMtMzg1ZjdiMWMwMzdm""
                    }
                  },
                  ""id"": ""e96b4442-879f-6601-8903-385f7b1c037f"",
                  ""uniqueName"": ""rreyzin@alegeus.com"",
                  ""imageUrl"": ""https://dev.azure.com/Alegeus-Technologies/_apis/GraphProfile/MemberAvatars/aad.ZTk2YjQ0NDItODc5Zi03NjAxLTg5MDMtMzg1ZjdiMWMwMzdm"",
                  ""descriptor"": ""aad.ZTk2YjQ0NDItODc5Zi03NjAxLTg5MDMtMzg1ZjdiMWMwMzdm""
                },
                ""System.ChangedDate"": ""2024-11-20T15:05:35.957Z"",
                ""System.ChangedBy"": {
                  ""displayName"": ""Roman Reyzin"",
                  ""url"": ""https://spsprodcus1.vssps.visualstudio.com/Adbba024b-4985-41b1-86e1-fb7a27d5055f/_apis/Identities/e96b4442-879f-6601-8903-385f7b1c037f"",
                  ""_links"": {
                    ""avatar"": {
                      ""href"": ""https://dev.azure.com/Alegeus-Technologies/_apis/GraphProfile/MemberAvatars/aad.ZTk2YjQ0NDItODc5Zi03NjAxLTg5MDMtMzg1ZjdiMWMwMzdm""
                    }
                  },
                  ""id"": ""e96b4442-879f-6601-8903-385f7b1c037f"",
                  ""uniqueName"": ""rreyzin@alegeus.com"",
                  ""imageUrl"": ""https://dev.azure.com/Alegeus-Technologies/_apis/GraphProfile/MemberAvatars/aad.ZTk2YjQ0NDItODc5Zi03NjAxLTg5MDMtMzg1ZjdiMWMwMzdm"",
                  ""descriptor"": ""aad.ZTk2YjQ0NDItODc5Zi03NjAxLTg5MDMtMzg1ZjdiMWMwMzdm""
                },
                ""System.CommentCount"": 0,
                ""System.Title"": ""[Schema]: PCI Schema Changes - Prior to 25.02 - udt_CARD_NUM_ENCRYPT: add new fields to udt"",
                ""System.BoardColumn"": ""New"",
                ""System.BoardColumnDone"": false,
                ""Microsoft.VSTS.Common.StateChangeDate"": ""2024-11-20T15:01:30.293Z"",
                ""Microsoft.VSTS.Common.Priority"": 2,
                ""Microsoft.VSTS.Common.ValueArea"": ""Tech Debt"",
                ""Microsoft.VSTS.Scheduling.TargetDate"": ""2025-01-18T05:00:00Z"",
                ""Custom.Release"": ""WC 24.10.00.P0"",
                ""Custom.Project"": ""WealthCare ART"",
                ""WEF_1490FD808907421CAB75EF525816639D_Kanban.Column"": ""New"",
                ""WEF_1490FD808907421CAB75EF525816639D_Kanban.Column.Done"": false,
                ""System.Description"": ""\u003Cdiv\u003E\u003Cbr\u003E \u003C/div\u003E\u003Cdiv\u003EAdd new fields to udf_CARD_NUM_ENCRYPT: \u003C/div\u003E\u003Cdiv\u003E\u003Cspan\u003E[bin_num]\t\t\t\tVARCHAR(6),\u003Cbr\u003E\u003C/span\u003E\u003Cdiv\u003E\t\t\t\t[next_card_num]\t\t\tVARCHAR(50)\u003Cbr\u003E \u003C/div\u003E\u003Cspan\u003E\u003C/span\u003E\u003Cbr\u003E \u003C/div\u003E\u003Cdiv\u003E\u003Cbr\u003E \u003C/div\u003E"",
                ""System.History"": """",
                ""System.Tags"": ""PCI V4 Req; Schema 2024PI4""
              },
              ""commentVersionRef"": {
                ""commentId"": 12827697,
                ""version"": 2,
                ""url"": ""https://dev.azure.com/Alegeus-Technologies/a61adc9c-ecf6-47ff-9a40-c5e3613f1c2f/_apis/wit/workItems/311351/comments/12827697/versions/2""
              },
              ""_links"": {
                ""self"": {
                  ""href"": ""https://dev.azure.com/Alegeus-Technologies/a61adc9c-ecf6-47ff-9a40-c5e3613f1c2f/_apis/wit/workItems/311351""
                },
                ""workItemUpdates"": {
                  ""href"": ""https://dev.azure.com/Alegeus-Technologies/a61adc9c-ecf6-47ff-9a40-c5e3613f1c2f/_apis/wit/workItems/311351/updates""
                },
                ""workItemRevisions"": {
                  ""href"": ""https://dev.azure.com/Alegeus-Technologies/a61adc9c-ecf6-47ff-9a40-c5e3613f1c2f/_apis/wit/workItems/311351/revisions""
                },
                ""workItemComments"": {
                  ""href"": ""https://dev.azure.com/Alegeus-Technologies/a61adc9c-ecf6-47ff-9a40-c5e3613f1c2f/_apis/wit/workItems/311351/comments""
                },
                ""html"": {
                  ""href"": ""https://dev.azure.com/Alegeus-Technologies/a61adc9c-ecf6-47ff-9a40-c5e3613f1c2f/_workitems/edit/311351""
                },
                ""workItemType"": {
                  ""href"": ""https://dev.azure.com/Alegeus-Technologies/a61adc9c-ecf6-47ff-9a40-c5e3613f1c2f/_apis/wit/workItemTypes/User%20Story""
                },
                ""fields"": {
                  ""href"": ""https://dev.azure.com/Alegeus-Technologies/a61adc9c-ecf6-47ff-9a40-c5e3613f1c2f/_apis/wit/fields""
                }
              },
              ""url"": ""https://dev.azure.com/Alegeus-Technologies/a61adc9c-ecf6-47ff-9a40-c5e3613f1c2f/_apis/wit/workItems/311351""
            }
            ";
            try
            {
                string replyText;

                // Check if input is valid JSON
                if (IsJson(userMessage))
                {
                    // Summarize JSON input
                    var engineName = "hackathon-1"; // Replace with your deployment name
                    replyText = await _gptClientHelper.SummarizeJsonAsync(engineName, userMessage);
                }
                else
                {
                    // Prepare a normal chat message for GPT
                    var chatMessage = new ChatRequestUserMessage(userMessage);
                    var engineName = "hackathon-1"; // Replace with your deployment name
                    var response = await _gptClientHelper.GetCompletionsAsync(engineName, chatMessage);

                    // Extract the reply text
                    replyText = response.Choices[0].Message.Content;
                }

                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                var errorMessage = $"An error occurred: {ex.Message}";

            }
        }

        /// <summary>
        /// Helper method to check if a string is valid JSON.
        /// </summary>
        /// <param name="input">Input string to validate.</param>
        /// <returns>True if the input is valid JSON, false otherwise.</returns>
        public bool IsJson(string input)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<object>(input);
                return obj != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
