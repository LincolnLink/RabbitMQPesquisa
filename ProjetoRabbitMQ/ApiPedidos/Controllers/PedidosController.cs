using ApiPedidos.Data;
using ApiPedidos.Models;
using ApiPedidos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPedidos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly PedidosContext _context;
    private readonly RabbitMqService _rabbitMq;

    public PedidosController(PedidosContext context, RabbitMqService rabbitMq)
    {
        _context = context;
        _rabbitMq = rabbitMq;
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        // Enviar evento para o RabbitMQ
        var evento = new PedidoCriadoEvent
        {
            PedidoId = pedido.Id,
            Produtos = pedido.Itens.Select(i => new ItemPedidoDto
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList()
        };

        _rabbitMq.PublicarPedidoCriado(evento);

        return CreatedAtAction(nameof(ObterPedido), new { id = pedido.Id }, pedido);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPedido(int id)
    {
        var pedido = await _context.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id);
        return pedido == null ? NotFound() : Ok(pedido);
    }
}
