using RabbitMQ.Client;
using RabbitMQ.Model;

const string Exchange_Principal_Pedido = "exchange_Principal_Pedido";
const string Queue_Principal_Pedido = "queue_Principal_Pedido";
const string RoutingKey_Principal_Pedido = "routingKey_Principal_Pedido";

const string DLX_Exchange_Dead_Letter = "exchange_Dead_Letter";
const string DLQ_Queue_Dead_Letter = "queue_Dead_Letter";
const string RoutingKey_DLX = "routingKey_Dead_Letter";

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
    exchange: Exchange_Principal_Pedido,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false
);
Console.WriteLine($"Exchange principal criado: {Exchange_Principal_Pedido}");

await channel.ExchangeDeclareAsync(
    exchange: DLX_Exchange_Dead_Letter,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false
);
Console.WriteLine($"DLX criado: {DLX_Exchange_Dead_Letter}");

await channel.QueueDeclareAsync(
    queue: DLQ_Queue_Dead_Letter,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);
Console.WriteLine($"DLQ criada: {DLQ_Queue_Dead_Letter}");

await channel.QueueBindAsync(
    queue: DLQ_Queue_Dead_Letter,
    exchange: DLX_Exchange_Dead_Letter,
    routingKey: RoutingKey_DLX
);
Console.WriteLine($"DLQ conectada à DLX com routing key: {RoutingKey_DLX}");

var mainQueueArgs = new Dictionary<string, object>
{
   {"x-dead-letter-exchange", DLX_Exchange_Dead_Letter },
   {"x-dead-letter-routing-key", RoutingKey_DLX }
};

await channel.QueueDeclareAsync(
    queue: Queue_Principal_Pedido,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: mainQueueArgs
);
Console.WriteLine($"Fila principal criada: {Queue_Principal_Pedido}");
Console.WriteLine($" └─ Configurada para usar DLX: {DLX_Exchange_Dead_Letter}");

await channel.QueueBindAsync(
    queue: Queue_Principal_Pedido,
    exchange: Exchange_Principal_Pedido,
    routingKey: RoutingKey_Principal_Pedido
);
Console.WriteLine($"Fila principal conectada ao exchange com routing key: {RoutingKey_Principal_Pedido}");

Console.WriteLine("===========================================");
Console.WriteLine();

Console.WriteLine("Quantos pedidos você quer enviar?");
if (!int.TryParse(Console.ReadLine(), out var quantidadePedidos))
{
    quantidadePedidos = 3;
}

Console.WriteLine();
Console.WriteLine("===========================================");
Console.WriteLine("📦 ENVIANDO PEDIDOS...");
Console.WriteLine("===========================================");

for (int i = 1; i <= quantidadePedidos; i++)
{
    var pedido = CriarPedidoErroFake(i);
    var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(pedido);

    var properties = new BasicProperties
    {
        Persistent = true,
        ContentType = "application/json",
        ContentEncoding = "utf-8",
        MessageId = pedido.Id.ToString(),
        Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
    };

    await channel.BasicPublishAsync(
        exchange: Exchange_Principal_Pedido,
        routingKey: RoutingKey_Principal_Pedido,
        mandatory: false,
        basicProperties: properties,
        body: body);

    Console.WriteLine($"✉️  Pedido {i} enviado:");
    Console.WriteLine($"   ID: {pedido.Id}");
    Console.WriteLine($"   Cliente: {pedido.ClienteEmail}");
    Console.WriteLine($"   Valor: {pedido.ValorTotal:C}");
    Console.WriteLine();

    if (i < quantidadePedidos)
    {
        Console.WriteLine("Pressione ENTER para enviar o próximo pedido...");
        Console.ReadLine();
    }
}

Console.WriteLine("===========================================");
Console.WriteLine("✅ Todos os pedidos foram enviados!");
Console.WriteLine("===========================================");

static Pedido CriarPedidoFake(int index)
{

    var valor = Random.Shared.Next(-100, 5000);
    return new Pedido
    {
        Id = Guid.NewGuid(),
        ClienteEmail = $"cliente{index}@email.com",
        ValorTotal = valor,
        DataCriacao = DateTime.UtcNow,
        Itens = new List<Item>
        {
            new Item
            {
                NomeProduto = $"Produto {index}",
                Quantidade = Random.Shared.Next(1, 5),
                PrecoUnitario = Random.Shared.Next(20, 1000)
            }
        }
    };
}


static Pedido CriarPedidoErroFake(int index)
{

    var valor = -100;
    return new Pedido
    {
        Id = Guid.NewGuid(),
        ClienteEmail = $"cliente{index}@email.com",
        ValorTotal = valor,
        DataCriacao = DateTime.UtcNow,
        Itens = new List<Item>
        {
            new Item
            {
                NomeProduto = $"Produto {index}",
                Quantidade = Random.Shared.Next(1, 5),
                PrecoUnitario = Random.Shared.Next(20, 1000)
            }
        }
    };
}