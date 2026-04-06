using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projeto.Data.Context;

namespace API_Principal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class PersonagemController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public PersonagemController(AppDbContext personagemDbContext)
        {
            _dbContext  = personagemDbContext;
        }
    }
}
