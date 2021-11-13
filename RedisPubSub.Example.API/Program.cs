using Microsoft.AspNetCore.Mvc;
using RedisPubSub.Example.API;

WebApplicationBuilder? builder = 
    WebApplication.CreateBuilder(args);

builder.Services
    .AddHostedService<ListennerHostedService>();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddRedisMessageBusService();

var app = builder.Build();
app.UseSwagger()
    .UseSwaggerUI()
    .UseHttpsRedirection();

app.MapPost("/events",
    async ([FromBody] string evt, [FromServices] IMessageBusService _messageBusService) 
        => {
            var total = await _messageBusService.Publish(new { Type = "ExampleIntegrationEvent", Content = evt });
            return Results.Ok(new { message = "published", subscribers = total }); 
        });

app.Run();
