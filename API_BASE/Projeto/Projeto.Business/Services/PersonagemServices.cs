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
            LimparErros(); // Limpa erros de operações anteriores

            if (!ExecutarValidacao(new PersonagemValidation(), personagem)) 
                return;

            await _personagemRepository.Adicionar(personagem);            
        }

        public Task Atualizar(Personagem personagem)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task Remover(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
