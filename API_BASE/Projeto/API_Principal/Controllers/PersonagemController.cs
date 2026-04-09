using API_Principal.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto.Business.Interfaces;
using Projeto.Business.Models;
using Projeto.Data.Context;

namespace API_Principal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class PersonagemController(
        AppDbContext appDbContext, 
        IPersonagemService personagemService,
        IMapper mapper
        ) : BaseController
    {
        private readonly AppDbContext _dbContext = appDbContext;
        private readonly IPersonagemService _personagemService = personagemService;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        public async Task<IActionResult> Adicionar(PersonagemViewModel personagemViewModel)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var personagem = _mapper.Map<Personagem>(personagemViewModel);
            await _personagemService.Adicionar(personagem);

            if (_personagemService.TemErros)
                return RespostaPersonalizada(_personagemService);

            // Retorna o objeto salvo com ID gerado
            var personagemRetorno = _mapper.Map<PersonagemViewModel>(personagem);
            return RespostaPersonalizada(_personagemService, personagemRetorno, "Personagem adicionado com sucesso!");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonagemViewModel>>> ObterTodos()
        {
            var personagens = await _dbContext.Personagens.ToListAsync();
            var personagensViewModel = _mapper.Map<IEnumerable<PersonagemViewModel>>(personagens);

            return Ok(new
            {
                sucesso = true,
                dados = personagensViewModel
            });
        }
    }
}
