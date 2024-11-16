using Measurement_Service.Repository;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using FeatureHubSDK;

namespace Measurement_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add the MySQL connection as a singleton service
            builder.Services.AddSingleton<MySqlConnection>(provider =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                return new MySqlConnection(connectionString);
            });

            // Add FeatureHub context with async initialization
            builder.Services.AddSingleton<IClientContext>(provider =>
            {
                var edgeUrl = "http://featurehub:8085";
                var sdkKey = "014435c7-e4ce-40a5-8559-01e0f730e202/Imib6Qfj2m0Cy4PfncwxjUrehQKs74gkldzdNOxp";

                var config = new EdgeFeatureHubConfig(edgeUrl, sdkKey);
                var clientContext = config.NewContext().Build().Result; // Initialize FeatureHub context and wait for the async result synchronously
                return clientContext;
            });

            // Register MeasurementRepository as scoped
            builder.Services.AddScoped<IMeasurementRepository, MeasurementRepository>();

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
        }
    }
}
