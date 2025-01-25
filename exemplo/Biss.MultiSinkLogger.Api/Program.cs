using Biss.MultiSinkLogger.ExceptionHandlers;
using Biss.MultiSinkLogger.Extensions;
using Biss.MultiSinkLogger.Http;
using Serilog;

namespace Biss.MultiSinkLogger.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configura o Serilog usando o LoggingManager
            LoggingManager.InitializeLogger(builder.Configuration);

            // Adiciona o Serilog ao Host
            builder.Host.UseSerilog();

            // Adiciona o HttpClient com o HttpLoggingHandler
            builder.Services.AddTransient<HttpLoggingHandler>();

            builder.Services.AddTransient<IExceptionHandler, DefaultExceptionHandler>();

            builder.Services.AddControllers();

            builder.Services.AddHttpClient("ExternalApi")
                .ConfigureHttpClient(client =>
                    {
                        client.DefaultRequestHeaders.UserAgent.TryParseAdd("request"); // Adiciona um User-Agent padrão
                    })
                .AddHttpMessageHandler<HttpLoggingHandler>();



            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Usa os middlewares de logging
            app.UseExceptionLogging();
            app.UseCustomLogging();

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
