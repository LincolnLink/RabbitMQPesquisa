using API_Principal.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Projeto.Business.Interfaces;
using Projeto.Business.Models;

namespace API_Principal.Controllers
{
    [Route("api/[controller]")]    
    public class PersonagemController(
        IPersonagemService personagemService,
        IMapper mapper
        ) : BaseController
    {
        private readonly IPersonagemService _personagemService = personagemService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonagemViewModel>>> ObterTodos()
        {
            var personagens = await _personagemService.ObterTodos();
            var personagensViewModel = _mapper.Map<IEnumerable<PersonagemViewModel>>(personagens);

            return Ok(new
            {
                sucesso = true,
                dados = personagensViewModel
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PersonagemViewModel>> ObterPorId(Guid id)
        {
            var personagem = await _personagemService.ObterPorId(id);

            if (personagem == null)
                return NotFound(new
                {
                    sucesso = false,
                    erros = new[] { "Personagem não encontrado." }
                });

            var personagemViewModel = _mapper.Map<PersonagemViewModel>(personagem);

            return Ok(new
            {
                sucesso = true,
                dados = personagemViewModel
            });
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar(PersonagemViewModel personagemViewModel)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var personagem = _mapper.Map<Personagem>(personagemViewModel);
            await _personagemService.Adicionar(personagem);

            if (_personagemService.TemErros)
                return RespostaPersonalizada(_personagemService);

            var personagemRetorno = _mapper.Map<PersonagemViewModel>(personagem);
            return CreatedAtAction(nameof(ObterPorId), new { id = personagem.Id }, new
            {
                sucesso = true,
                mensagem = "Personagem adicionado com sucesso!",
                dados = personagemRetorno
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, PersonagemViewModel personagemViewModel)
        {
            if (id != personagemViewModel.Id)
                return BadRequest(new
                {
                    sucesso = false,
                    erros = new[] { "O ID informado não é o mesmo que foi passado na query." }
                });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var personagem = _mapper.Map<Personagem>(personagemViewModel);
            await _personagemService.Atualizar(personagem);

            if (_personagemService.TemErros)
                return RespostaPersonalizada(_personagemService);

            return RespostaPersonalizada(_personagemService, personagemViewModel, "Personagem atualizado com sucesso!");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Remover(Guid id)
        {
            var personagemViewModel = await _personagemService.ObterPorId(id);

            if (personagemViewModel == null)
                return NotFound(new
                {
                    sucesso = false,
                    erros = new[] { "Personagem não encontrado." }
                });

            await _personagemService.Remover(id);

            if (_personagemService.TemErros)
                return RespostaPersonalizada(_personagemService);

            return RespostaPersonalizada(_personagemService, null, "Personagem removido com sucesso!");
        }
    }
}
