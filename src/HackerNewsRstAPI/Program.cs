using HackerNewsRstAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Configuration;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHackerNews(builder.Configuration).AddRedisCache(builder.Configuration);
builder.Services.AddLogging();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var controllerActionDescriptor = context
        .GetEndpoint()?
        .Metadata
        .GetMetadata<ControllerActionDescriptor>();

    var controllerName = $"{controllerActionDescriptor?.ControllerName}Controller";

    if (controllerName is not nameof(HackerNewsController))
    {
        await next();
        return;
    }

    Stopwatch stopwatch = Stopwatch.StartNew();

    context.Response.OnStarting(() =>
    {
        stopwatch.Stop();
        context.Response.Headers["time"] = $"{stopwatch.ElapsedMilliseconds} ms";
        return Task.CompletedTask;
    });

    await next();
});

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
