using Projeto.Business.Interfaces;
using Projeto.Business.Models;
using Projeto.Data.Context;

namespace Projeto.Data.Repository
{
    public class PersonagemRepository : Repository<Personagem>, IPersonagemRepository
    {
        public PersonagemRepository(AppDbContext db) : base(db){}

        //implementação caso tenha outro metodo alem do metod padrão do repository.
    }
}
