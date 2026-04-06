using API_Principal.Models;
using Microsoft.EntityFrameworkCore;


namespace Projeto.Data.Context
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options) : base(options){}
        public DbSet<Personagem> Personagens { get; set; }
    }
}
