namespace Projeto.Business.Interfaces
{
    public interface IBaseService
    {
        IReadOnlyCollection<string> Erros { get; }
        bool TemErros { get; }
    }
}
