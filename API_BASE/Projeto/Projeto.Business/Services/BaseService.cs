using FluentValidation;
using FluentValidation.Results;
using Projeto.Business.Interfaces;
using Projeto.Business.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Projeto.Business.Services
{
    public class BaseService : IBaseService
    {
        private List<string> _erros = new List<string>();

        public IReadOnlyCollection<string> Erros => _erros.AsReadOnly();

        public bool TemErros => _erros.Any();

        protected void LimparErros()
        {
            _erros.Clear();
        }

        protected void AdicionarErro(string mensagem)
        {
            _erros.Add(mensagem);
        }

        protected bool ExecutarValidacao<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity
        {
            var validator = validacao.Validate(entidade);

            if (validator.IsValid) return true;

            foreach (var error in validator.Errors)
            {
                AdicionarErro(error.ErrorMessage);
            }

            return false;
        }
    }
}
