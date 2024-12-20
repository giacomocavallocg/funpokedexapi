
using System.Net;
using FunPokedex.Api.Exceptions;
using FunPokedex.Application.Infrastructure;
using FunPokedex.Application.Interfaces;
using FunPokedex.Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IPokeApi, PokeApi>(c =>
{
    c.BaseAddress = new Uri("https://pokeapi.co");
}).AddResilienceHandler(
    "PokeApiPipeline",
    static builder =>
    {
        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            SamplingDuration = TimeSpan.FromSeconds(10),
            FailureRatio = 0.2,
            MinimumThroughput = 3,
            BreakDuration = TimeSpan.FromMinutes(1),
            ShouldHandle = static args =>
            {
                if (args.Outcome.Exception != null) return ValueTask.FromResult(true);

                if (args.Outcome.Result != null &&
                (args.Outcome.Result.StatusCode == HttpStatusCode.TooManyRequests || (int)args.Outcome.Result.StatusCode >= 500))
                {
                    // status code >= 500 or 429
                    return ValueTask.FromResult(true);
                }
                return ValueTask.FromResult(false);
            },
        });
    });


builder.Services.AddHttpClient<IFunTranslationsApi, FunTranslationApi>(c =>
{
    c.BaseAddress = new Uri("https://api.funtranslations.com");
}).AddResilienceHandler(
    "FunTranslationApiPipeline",
    static builder =>
    {
        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            SamplingDuration = TimeSpan.FromSeconds(10),
            FailureRatio = 0.2,
            MinimumThroughput = 3,
            BreakDuration = TimeSpan.FromMinutes(1),
            ShouldHandle = static args =>
            {
                if (args.Outcome.Exception != null) return ValueTask.FromResult(true);

                if (args.Outcome.Result != null &&
                (args.Outcome.Result.StatusCode == HttpStatusCode.TooManyRequests || (int)args.Outcome.Result.StatusCode >= 500))
                {
                    // status code >= 500 or 429
                    return ValueTask.FromResult(true);
                }
                return ValueTask.FromResult(false);
            },
        });
    });

builder.Services.AddSingleton<IApplicationCache>(_ => new ApplicationMemoryCache(new MemoryCache(new MemoryCacheOptions()), TimeSpan.FromMinutes(10)));

builder.Services.AddScoped<IPokedexService, PokedexService>();
builder.Services.AddScoped<IFunPokedexService, FunPokedexService>();

// add exceptions handlers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
});


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
