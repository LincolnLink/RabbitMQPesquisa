using Microsoft.EntityFrameworkCore;
using Projeto.Business.Models;

namespace Projeto.Data.Mappings
{
    internal class PersonagemMapping : IEntityTypeConfiguration<Personagem>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Personagem> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnType("uuid")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasColumnType("varchar(100)")
                .HasMaxLength(100);

            builder.Property(p => p.Tipo)
                .IsRequired()
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);

            builder.ToTable("Personagens");
        }
    }
}
