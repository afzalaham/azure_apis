using gpt4.Bots;
using Hackathon.Azdo.KPI.Analyzer.API.Services.GPT_4;
using Hackathon.Azdo.KPI.Analyzer.API.Services.GPT_4.Interface;
using Hackathon.Azdo.KPI.Gpt4.Services;
using Hackathon.Azdo.KPI.Gpt4.Services.Interface;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IsupportBot, supportBot>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc().AddNewtonsoftJson();
builder.Services.AddSingleton<IBuildService, BuildService>();
#pragma warning disable CS0618
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
#pragma warning restore CS0618 
builder.Services.AddSingleton<IGPT4ClientHelper, GPT4ClientHelper>();
builder.Services.AddSingleton<IPullRequestService, PullRequestService>();

builder.Services.AddSingleton<IWikiDocumentService, WikiDocumentService>();
builder.Services.AddSingleton<ReleaseService>();
builder.Services.AddSingleton<SprintIterationService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
