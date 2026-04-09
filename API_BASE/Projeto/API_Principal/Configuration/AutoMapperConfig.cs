using AutoMapper;
using API_Principal.ViewModels;
using Projeto.Business.Models;

namespace API_Principal.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // ViewModel -> Entity
            CreateMap<PersonagemViewModel, Personagem>();

            // Entity -> ViewModel
            CreateMap<Personagem, PersonagemViewModel>();
        }
    }
}
