using ApiPedidos.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPedidos.Data;

public class PedidosContext : DbContext
{
    public PedidosContext(DbContextOptions<PedidosContext> options) : base(options) { }

    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> Itens => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Itens)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
