using Microsoft.EntityFrameworkCore;
using Projeto.Business.Models;


namespace Projeto.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<Personagem> Personagens { get; set; }
    }
}
