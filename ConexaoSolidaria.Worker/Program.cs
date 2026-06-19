using ConexaoSolidaria.Worker.Infrastructure.Extensions;
using ConexaoSolidaria.Worker.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataBaseContext(builder.Configuration);
builder.Services.AddRepository();
builder.Services.AddBus(builder.Configuration);
builder.Services.AddJobs();
builder.Services.AddHealthChecks();

var host = builder.Build();
host.UseHealthChecks("/health");
host.AddMetrics();

host.Run();
