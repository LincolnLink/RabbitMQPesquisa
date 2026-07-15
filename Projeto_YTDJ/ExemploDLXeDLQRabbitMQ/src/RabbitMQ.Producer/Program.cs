using RabbitMQ.Client;
using RabbitMQ.Model;

const string pedidoExchangeName = "pedidoCriado.exchange";
const string pedidoQueueName = "pedidoCriado.queue";
const string pedidoRoutingKey = "pedidoCriado.routingKey";

const string pedidoDLXName = "pedido.deadLetter.exchange";
const string pedidoDLQName = "pedido.deadLetter.queue";
const string pedidoDLXRoutingKey = "pedido.deadLetter.routingKey";

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/",
    AutomaticRecoveryEnabled = true,
    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

Console.WriteLine("===========================================");
Console.WriteLine("🚀 CONFIGURANDO EXCHANGES E FILAS...");
Console.WriteLine("===========================================");

await channel.ExchangeDeclareAsync(
    exchange: pedidoExchangeName,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false
);
Console.WriteLine($"Exchange principal criado: {pedidoExchangeName}");

await channel.ExchangeDeclareAsync(
    exchange: pedidoDLXName,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false
);
Console.WriteLine($"DLX criado: {pedidoDLXName}");

await channel.QueueDeclareAsync(
    queue: pedidoDLQName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);
Console.WriteLine($"DLQ criada: {pedidoDLQName}");

await channel.QueueBindAsync(
    queue: pedidoDLQName,
    exchange: pedidoDLXName,
    routingKey: pedidoDLXRoutingKey
);
Console.WriteLine($"DLQ conectada à DLX com routing key: {pedidoDLXRoutingKey}");

var mainQueueArgs = new Dictionary<string, object>
{
   {"x-dead-letter-exchange", pedidoDLXName },
   {"x-dead-letter-routing-key", pedidoDLXRoutingKey }
};

await channel.QueueDeclareAsync(
    queue: pedidoQueueName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: mainQueueArgs
);
Console.WriteLine($"Fila principal criada: {pedidoQueueName}");
Console.WriteLine($" └─ Configurada para usar DLX: {pedidoDLXName}");

await channel.QueueBindAsync(
    queue: pedidoQueueName,
    exchange: pedidoExchangeName,
    routingKey: pedidoRoutingKey
);
Console.WriteLine($"Fila principal conectada ao exchange com routing key: {pedidoRoutingKey}");


Console.WriteLine("===========================================");
Console.WriteLine();