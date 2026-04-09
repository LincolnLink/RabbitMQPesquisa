using Projeto.Business.Interfaces;
using Projeto.Business.Models;
using Projeto.Business.Models.Validations;

namespace Projeto.Business.Services
{
    public class PersonagemServices : BaseService, IPersonagemService
    {
        private readonly IPersonagemRepository _personagemRepository;

        public PersonagemServices(IPersonagemRepository personagemRepository)
        {
            _personagemRepository = personagemRepository;
        }

        public async Task Adicionar(Personagem personagem)
        {
            LimparErros();

            if (!ExecutarValidacao(new PersonagemValidation(), personagem)) 
                return;

            await _personagemRepository.Adicionar(personagem);            
        }

        public async Task Atualizar(Personagem personagem)
        {
            LimparErros();

            if (!ExecutarValidacao(new PersonagemValidation(), personagem))
                return;

            await _personagemRepository.Atualizar(personagem);
        }

        public async Task Remover(Guid id)
        {
            LimparErros();

            var personagem = await _personagemRepository.ObterPorId(id);

            if (personagem == null)
            {
                AdicionarErro("Personagem não encontrado.");
                return;
            }

            await _personagemRepository.Remover(id);
        }

        public async Task<Personagem?> ObterPorId(Guid id)
        {
            return await _personagemRepository.ObterPorId(id);
        }

        public async Task<IEnumerable<Personagem>> ObterTodos()
        {
            return await _personagemRepository.ObterTodos();
        }

        public void Dispose()
        {
            _personagemRepository?.Dispose();
        }
    }
}
