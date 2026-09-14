using Azure.Monitor.OpenTelemetry.Exporter;
using Hegerovi.Backend.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

var dataProvider = builder.Configuration["DataProvider"] ?? "InMemory";

if (dataProvider.Equals("Cosmos", StringComparison.OrdinalIgnoreCase))
{
    var cosmosConnectionString = builder.Configuration["CosmosDbConnectionString"]
        ?? throw new InvalidOperationException("CosmosDbConnectionString is not configured.");
    var cosmosDatabaseName = builder.Configuration["CosmosDbDatabaseName"] ?? "Hegerovi";

    builder.Services.AddSingleton(_ =>
    {
        var options = new CosmosClientOptions { ConnectionMode = ConnectionMode.Gateway };

        if (cosmosConnectionString.Contains("localhost", StringComparison.OrdinalIgnoreCase))
        {
            options.HttpClientFactory = () => new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
        }

        return new CosmosClient(cosmosConnectionString, options);
    });
    builder.Services.AddSingleton<IDataStore>(sp => new CosmosDataStore(sp.GetRequiredService<CosmosClient>(), cosmosDatabaseName));

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var cosmosClient = scope.ServiceProvider.GetRequiredService<CosmosClient>();
        await CosmosDataStore.EnsureCreatedAsync(cosmosClient, cosmosDatabaseName);
    }

    app.Run();
}
else
{
    builder.Services.AddSingleton<IDataStore, InMemoryDataStore>();
    builder.Build().Run();
}
