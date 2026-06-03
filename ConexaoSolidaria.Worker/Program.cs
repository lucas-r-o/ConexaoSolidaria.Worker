using ConexaoSolidaria.Worker.Infrastructure.Extensions;
using ConexaoSolidaria.Worker.Infrastructure.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDataBaseContext(builder.Configuration);
builder.Services.AddRepository();
builder.Services.AddBus(builder.Configuration);
builder.Services.AddJobs();

var host = builder.Build();
host.Run();
