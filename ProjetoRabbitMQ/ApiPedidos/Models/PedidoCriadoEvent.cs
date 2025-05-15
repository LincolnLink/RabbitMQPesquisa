namespace ApiPedidos.Models
{
    public class PedidoCriadoEvent
    {
        public int PedidoId { get; set; }
        public List<ItemPedidoDto> Produtos { get; set; } = new();
    }

    public class ItemPedidoDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}
