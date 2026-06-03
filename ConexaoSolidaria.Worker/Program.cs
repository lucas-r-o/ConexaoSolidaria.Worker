using ConexaoSolidaria.Worker.Infrastructure.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDataBaseContext(builder.Configuration);
builder.Services.AddBus(builder.Configuration);

var host = builder.Build();
host.Run();
