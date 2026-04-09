using Microsoft.AspNetCore.Mvc;
using Projeto.Business.Interfaces;

namespace API_Principal.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult RespostaPersonalizada(IBaseService service, object resultado = null, string mensagem = null)
        {
            if (service.TemErros)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    erros = service.Erros
                });
            }

            return Ok(new
            {
                sucesso = true,
                mensagem = mensagem ?? "Operação realizada com sucesso!",
                dados = resultado
            });
        }

        protected IActionResult RespostaErro(string mensagem)
        {
            return BadRequest(new
            {
                sucesso = false,
                erros = new[] { mensagem }
            });
        }
    }
}
