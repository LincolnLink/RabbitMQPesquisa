using Microsoft.EntityFrameworkCore;
using Projeto.Business.Interfaces;
using Projeto.Business.Models;
using Projeto.Data.Context;
using System.Linq.Expressions;

namespace Projeto.Data.Repository
{
    public abstract class Repository<T>(AppDbContext db) : IRepository<T> where T : Entity, new()
    {
        protected readonly AppDbContext Db = db;
        protected readonly DbSet<T> DbSet = db.Set<T>();

        public async Task<IEnumerable<T>> Buscar(Expression<Func<T, bool>> predicate)
        {
            // Percebe as mudanças de estado, retorna as mudanças com mais performace.
            // Deve sempre usar o await, para receber o valor do banco.
            // AsNoTracking: pesquisar melhor, sei que serve para não da bug.
            // predicate: é uma "expressão" que é uma função que retorna um valor bool.
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> ObterPorId(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> ObterTodos()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task Adicionar(T entity)
        {
            DbSet.Add(entity);
            await SaveChanges();
        }

        public virtual async Task Atualizar(T entity)
        {
            DbSet.Update(entity);
            await SaveChanges();
        }

        public virtual async Task Remover(Guid id)
        {
            DbSet.Remove(new T { Id = id });
            await SaveChanges();
        }

        /// <summary>
        /// Salva no banco do contexto.
        /// Caso tenha algum tratamento, faça em apenas um método.
        /// </summary>        
        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
