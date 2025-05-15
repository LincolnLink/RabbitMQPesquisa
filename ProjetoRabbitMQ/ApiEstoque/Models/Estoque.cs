namespace ApiEstoque.Models;

public class Estoque
{
    public int Id { get; set; }
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}
