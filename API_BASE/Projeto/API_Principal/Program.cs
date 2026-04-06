using Microsoft.EntityFrameworkCore;
using Projeto.Data.Context;

namespace API_Principal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 40));

            builder.Services.AddDbContext<PersonagemDbContext>(options =>
                options.UseMySql(connectionString, serverVersion));

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/", () => Results.Redirect("/swagger"))
               .ExcludeFromDescription();

            app.MapControllers();

            app.Run();
        }
    }
}
