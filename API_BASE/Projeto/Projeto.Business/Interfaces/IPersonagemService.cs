using Projeto.Business.Models;

namespace Projeto.Business.Interfaces
{
    public interface IPersonagemService : IBaseService, IDisposable
    {
        Task Adicionar(Personagem personagem);
        Task Atualizar(Personagem personagem);
        Task Remover(Guid id);
        Task<Personagem?> ObterPorId(Guid id);
        Task<IEnumerable<Personagem>> ObterTodos();
    }
}
