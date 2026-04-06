using API_Principal.Models;
using Microsoft.EntityFrameworkCore;


namespace Projeto.Data.Context
{
    public class PersonagemDbContext : DbContext
    {
        public PersonagemDbContext(DbContextOptions options) : base(options){}

        public DbSet<Personagem> BancoPrincipal { get; set; }
    }
}
