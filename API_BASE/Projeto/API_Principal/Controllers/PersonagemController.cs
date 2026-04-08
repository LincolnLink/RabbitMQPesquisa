using API_Principal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public async Task<IActionResult> Adicionar(Personagem personagem)
        {
            _dbContext.Personagens.Add(personagem);
            await _dbContext.SaveChangesAsync();
            return Ok(personagem);
            //return CreatedAtAction(nameof(ObterPorId), new { id = personagem.Id }, personagem);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personagem>>> ObterTodos()
        {
            var personagens = await _dbContext.Personagens.ToListAsync();
            return Ok(personagens);
        }
    }
}
