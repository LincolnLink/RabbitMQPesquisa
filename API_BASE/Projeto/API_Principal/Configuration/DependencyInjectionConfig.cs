using Projeto.Business.Interfaces;
using Projeto.Business.Services;
using Projeto.Data.Context;
using Projeto.Data.Repository;

namespace API_Principal.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<AppDbContext>();

            // Repositories
            services.AddScoped<IPersonagemRepository, PersonagemRepository>();
            //services.AddScoped<IProdutoRepository, ProdutoRepository>();
            //services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            //services.AddScoped<IEnderecoRepository, EnderecoRepository>();

            // Services
            services.AddScoped<IPersonagemService, PersonagemServices>();
            //services.AddScoped<IFornecedorService, FornecedorService>();
            //services.AddScoped<IProdutoService, ProdutoService>();

            //services.AddScoped<INotificador, Notificador>();

            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<IUser, AspNetUser>();

            //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }
}
