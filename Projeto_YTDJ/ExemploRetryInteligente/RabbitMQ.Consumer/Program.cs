using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Model;
using System.Text;
using System.Text.Json;

const string Exchange_Principal_Pedido = "exchange_Principal_Pedido";
const string Queue_Principal_Pedido = "queue_Principal_Pedido";
const string RoutingKey_Principal_Pedido = "routingKey_Principal_Pedido";

const string DLX_Exchange_Dead_Letter = "exchange_Dead_Letter";
const string DLQ_Queue_Dead_Letter = "queue_Dead_Letter";
const string RoutingKey_DLX = "routingKey_Dead_Letter";

const string retryExchange = "exchange_retry";
const string retryQueue = "queue_retry";
const string retryRoutingKey = "routingKey_retry";

const int maxRetryAttempsts = 3;
const int retryDelayMilliseconds = 5000;

const bool simular = false;

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

await channel.ExchangeDeclareAsync(Exchange_Principal_Pedido, ExchangeType.Direct, durable: true, autoDelete: false);
await channel.ExchangeDeclareAsync(DLX_Exchange_Dead_Letter, ExchangeType.Direct, durable: true, autoDelete: false);
await channel.ExchangeDeclareAsync(retryExchange, ExchangeType.Direct, durable: true, autoDelete: false);

await channel.QueueDeclareAsync(DLQ_Queue_Dead_Letter, durable: true, exclusive: false, autoDelete: false, arguments: null);
await channel.QueueBindAsync(DLQ_Queue_Dead_Letter, DLX_Exchange_Dead_Letter, RoutingKey_DLX);

var retryQueueArguments = new Dictionary<string, object>
{
    { "x-message-ttl", retryDelayMilliseconds },
    { "x-dead-letter-exchange", Exchange_Principal_Pedido },
    { "x-dead-letter-routing-key", RoutingKey_Principal_Pedido },    
};

await channel.QueueDeclareAsync(retryQueue, durable: true, exclusive: false, autoDelete: false, arguments: retryQueueArguments);
Console.WriteLine($"Fila Retry: '{retryQueue}' (TTL: {retryDelayMilliseconds}ms)");

await channel.QueueBindAsync(retryQueue, retryExchange, retryRoutingKey);
Console.WriteLine();

var mainQueueArguments = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", DLX_Exchange_Dead_Letter },
    { "x-dead-letter-routing-key", RoutingKey_DLX }
};

await channel.QueueDeclareAsync(Queue_Principal_Pedido, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArguments);
await channel.QueueBindAsync(Queue_Principal_Pedido, Exchange_Principal_Pedido, RoutingKey_Principal_Pedido);
Console.WriteLine();

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
Console.WriteLine();

Console.WriteLine("===========================================");
Console.WriteLine("🔧 Aguardando Mensagem ...");
Console.WriteLine("===========================================");
Console.WriteLine();

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (_, ea) =>
{
    var body = ea.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);

    int retryConunt = 0;
    if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.ContainsKey("x-retry-count"))
    {
        retryConunt = Convert.ToInt32(ea.BasicProperties.Headers["x-retry-count"]);
    }

    Console.WriteLine($"Tentativa {retryConunt + 1}/{maxRetryAttempsts}");

    try
    {
        var pedido = JsonSerializer.Deserialize<Pedido>(json);

        Console.WriteLine($" Id: {pedido?.Id}");
        Console.WriteLine($" Cliente: {pedido?.ClienteEmail}");
        Console.WriteLine($" Valor: {pedido?.ValorTotal}");
        Console.WriteLine();

        if (pedido == null)
        {
            throw new InvalidOperationException("X Pedido veio nulo!");
        }

        if(pedido.ValorTotal < 0)
        {
            Console.WriteLine("X Valor negativo -> DLQ");
            await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            return;
        }
        if(string.IsNullOrWhiteSpace(pedido.ClienteEmail))
        {
            Console.WriteLine("X ClienteEmail vazio -> DLQ");
            await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            return;
        }

        Console.WriteLine("SUCESSO");
        Console.WriteLine();

        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

    }
    catch (JsonException ex)
    {
        Console.WriteLine($"X Json corrompido: {ex.Message} -> DLQ");
        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
    }
    catch(Exception ex)
    {
        Console.WriteLine($" X Erro temporário: {ex.Message}");

        if(retryConunt < maxRetryAttempsts - 1)
        {
            var newRetryCount = retryConunt + 1;

            var headers = new Dictionary<string, object?>
            {
                { "x-retry-count", newRetryCount }
            };

            var retryProperties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                ContentEncoding = "utf-8",
                Headers = headers
            };

            await channel.BasicPublishAsync(
                exchange: retryExchange,
                routingKey: retryRoutingKey,
                mandatory: false,
                basicProperties: retryProperties,
                body: body);

            Console.WriteLine($"Enviado para retry(tentativa {newRetryCount + 1} em {retryDelayMilliseconds}ms)");
            Console.WriteLine();

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        }
        else
        {
            Console.WriteLine($"X Maximo de tentativas atingido: {ex.Message} -> DLQ");
            Console.WriteLine();
            await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
        }

    }
};

await channel.BasicConsumeAsync(queue: Queue_Principal_Pedido, autoAck: false, consumer: consumer);

Console.WriteLine("Pressione [enter] para parar.");
Console.ReadLine();
