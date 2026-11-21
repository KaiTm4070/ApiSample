using System.Text.Json.Serialization;
using ApiSampleFinal.Middleware;
using Infrastructure;
using Microsoft.OpenApi.Models;
using Services;

namespace ApiSampleFinal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Raíz de composición: aquí se conectan las capas (Web → Services → Infrastructure → Domain).
            builder.Services.AddServices();
            builder.Services.AddRepositories(builder.Configuration);

            builder.Services.AddControllers()
                // Los enums viajan como texto ("Queso", "Yogurt") y no como números.
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Manejo global de errores: toda excepción termina como ProblemDetails con su código HTTP.
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            // Swagger / OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Granja Lechera",
                    Version = "v1",
                    Description = "Producción de leche, transformación en queso/yogurt, inventario por lotes, ventas FIFO y reportes."
                });
                c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date", Example = new Microsoft.OpenApi.Any.OpenApiString("2026-12-01") });

                var xml = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
                if (File.Exists(xml)) c.IncludeXmlComments(xml);
            });

            //configuracion cors
            builder.Services.AddCors(p => p.AddPolicy("CORS_Policy", policy =>
            {
                policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));

            var app = builder.Build();

            // Aplica migraciones y crea la granja inicial si la base está vacía.
            await app.Services.InicializarBaseDeDatosAsync();

            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //aplica cors policy
            app.UseCors("CORS_Policy");

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
