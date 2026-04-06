using API_Principal.Models;
using Microsoft.EntityFrameworkCore;


namespace Projeto.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<Personagem> Personagens { get; set; }
    }
}
