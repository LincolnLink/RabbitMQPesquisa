using RabbitMQ.Client;

// Configurando a fábrica de conexões
var factory = new ConnectionFactory
{ 
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/",
    AutomaticRecoveryEnabled = true,
    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
};

// Criando conexão e canal
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

// Criando uma fila persistente
await channel.QueueDeclareAsync(
    queue: "persistent_queue",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

// Criando mensagem persistente
var message = "Mensagem de Exemplo";
var body = System.Text.Encoding.UTF8.GetBytes(message);

// Configurando a propriedade de persistência da mensagem
var properties = new BasicProperties
{
    Persistent = true
};

// Publicando a mensagem na fila persistente
await channel.BasicPublishAsync(
    exchange: "", //default exchange
    routingKey: "persistent_queue",
    mandatory: false,
    basicProperties: properties,
    body: body
);

Console.WriteLine("Mensagem publicada na fila persistente: " + message);