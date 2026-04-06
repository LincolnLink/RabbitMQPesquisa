using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projeto.Data.Context;

namespace API_Principal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class PersonagemController : ControllerBase
    {
        private readonly DbContext _dbContext;

        public PersonagemController(DbContext personagemDbContext)
        {
            _dbContext  = personagemDbContext;
        }
    }
}
