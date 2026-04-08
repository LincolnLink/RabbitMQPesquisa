using Microsoft.EntityFrameworkCore;
using Projeto.Data.Context;

namespace API_Principal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // SEMPRE ativa o Swagger (não só em Development)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Principal v1");
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapGet("/", () => Results.Redirect("/swagger"))
               .ExcludeFromDescription();

            app.MapGet("/health", () => Results.Ok("API está funcionando!"))
               .WithTags("Health");

            app.MapControllers();

            app.Run();
        }
    }
}
