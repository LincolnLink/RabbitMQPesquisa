using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto.Data.Context;
using Projeto.Business.Models;
using Projeto.Business.Interfaces;

namespace API_Principal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class PersonagemController(AppDbContext appDbContext, IPersonagemService personagemService) : BaseController
    {
        private readonly AppDbContext _dbContext = appDbContext;
        private readonly IPersonagemService _personagemService = personagemService;

        [HttpPost]
        public async Task<IActionResult> Adicionar(Personagem personagem)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            await _personagemService.Adicionar(personagem);

            return RespostaPersonalizada(_personagemService, personagem, "Personagem adicionado com sucesso!");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personagem>>> ObterTodos()
        {
            var personagens = await _dbContext.Personagens.ToListAsync();

            return Ok(new
            {
                sucesso = true,
                dados = personagens
            });
        }
    }
}
