using ConexaoSolidaria.Worker.Infrastructure.Consumers;
using MassTransit;

namespace ConexaoSolidaria.Worker.Infrastructure.Extensions
{
    internal static class BusExtensions
    {
        private const string SECTION_BUS = "Bus";
        private const string SUBSECTION_PORT = "Port";
        private const string SUBSECTION_HOST = "Host";
        private const string SUBSECTION_USER = "User";
        private const string SUBSECTION_PASSWORD = "Password";

        public static void AddBus(this IServiceCollection services, IConfigurationManager config)
        {
            var busSection = config.GetSection(SECTION_BUS);

            var host = busSection.GetRequiredSection(SUBSECTION_HOST)?.Value ?? throw new ArgumentNullException(nameof(SUBSECTION_HOST));
            var port = busSection.GetRequiredSection(SUBSECTION_PORT)?.Value ?? throw new ArgumentNullException(nameof(SUBSECTION_PORT));
            var user = busSection.GetRequiredSection(SUBSECTION_USER)?.Value ?? throw new ArgumentNullException(nameof(SUBSECTION_USER));
            var password = busSection.GetRequiredSection(SUBSECTION_PASSWORD)?.Value ?? throw new ArgumentNullException(nameof(SUBSECTION_PASSWORD));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<DoacaoRecebidaConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseJsonSerializer();
                    cfg.ConfigureJsonSerializerOptions(options =>
                    {
                        options.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.Strict;
                        return options;
                    });

                    cfg.Host(host, ushort.Parse(port), "/", h =>
                    {
                        h.Username(user);
                        h.Password(password);
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
