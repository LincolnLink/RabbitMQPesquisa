using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ApiPedidos.Models;


namespace ApiPedidos.Services;

public class RabbitMqService
{
    private readonly IModel _channel;

    public RabbitMqService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection(); // <-- Aqui é onde estava o erro
        _channel = connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "eventos", type: ExchangeType.Topic);
    }

    public void PublicarPedidoCriado(PedidoCriadoEvent pedido)
    {
        var message = JsonSerializer.Serialize(pedido);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "eventos",
            routingKey: "pedido.criado",
            basicProperties: null,
            body: body
        );
    }
}
