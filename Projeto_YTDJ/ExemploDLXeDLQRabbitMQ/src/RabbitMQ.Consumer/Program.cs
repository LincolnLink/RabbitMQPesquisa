using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

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
Console.WriteLine("🔧 CONFIGURANDO CONSUMER...");
Console.WriteLine("===========================================");

// GARANTIR QUE TUDO ESTÁ CRIADO (caso rode antes do Producer)

// Exchange principal
await channel.ExchangeDeclareAsync(
    exchange: pedidoExchangeName,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false
);

// DLX
await channel.ExchangeDeclareAsync(
    exchange: pedidoDLXName,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false
);

// DLQ
await channel.QueueDeclareAsync(
    queue: pedidoDLQName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

// Bind DLQ à DLX
await channel.QueueBindAsync(
    queue: pedidoDLQName,
    exchange: pedidoDLXName,
    routingKey: pedidoDLXRoutingKey
);

// Fila Principal com DLX configurada
var mainQueueArgs = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", pedidoDLXName },
    { "x-dead-letter-routing-key", pedidoDLXRoutingKey }
};

await channel.QueueDeclareAsync(
    queue: pedidoQueueName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: mainQueueArgs
);

// Bind fila principal à exchange principal
await channel.QueueBindAsync(
    queue: pedidoQueueName,
    exchange: pedidoExchangeName,
    routingKey: pedidoRoutingKey
);

// Qos - processar 1 mensagem por vez
await channel.BasicQosAsync(
    prefetchSize: 0, 
    prefetchCount: 1, 
    global: false
);

Console.WriteLine("✅ Configuração completa!");
Console.WriteLine("===========================================");
Console.WriteLine();
Console.WriteLine("👂 Aguardando mensagens...");
Console.WriteLine("===========================================");
Console.WriteLine();

// Consumer
var consumer = new AsyncEventingBasicConsumer(channel);

