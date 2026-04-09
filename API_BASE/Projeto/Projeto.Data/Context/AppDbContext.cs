using Microsoft.EntityFrameworkCore;
using Projeto.Business.Models;


namespace Projeto.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<Personagem> Personagens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Personagem>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .ValueGeneratedOnAdd();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
