using ApiEstoque.Data;
using ApiEstoque.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ApiEstoque.Services;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqConsumerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "pedidos-exchange", type: ExchangeType.Direct);
        _channel.QueueDeclare(queue: "estoque-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queue: "estoque-queue", exchange: "pedidos-exchange", routingKey: "pedido-criado");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var pedido = JsonSerializer.Deserialize<PedidoMessage>(message);

            if (pedido is not null)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<EstoqueContext>();

                var produto = await db.Estoques.FirstOrDefaultAsync(e => e.Produto == pedido.Produto);

                if (produto != null)
                {
                    produto.Quantidade -= pedido.Quantidade;
                    await db.SaveChangesAsync();
                    Console.WriteLine($" Estoque atualizado para {produto.Produto}: {produto.Quantidade}");
                }
                else
                {
                    Console.WriteLine($" Produto não encontrado: {pedido.Produto}");
                }
            }
        };

        _channel.BasicConsume(queue: "estoque-queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
