# RabbitMQPesquisa

- Desenvolvendo uma API base com Microsoft.EntityFrameworkCore.

- Estudo sobre RabbitMQ , exemplo pratico de como funciona.

- DotnetCore 9. com PostgreeMSQL

# Referencia

 - Referencia da aula de rabbitMQ:

  https://www.youtube.com/watch?v=FXNSnsVQafE

 - O Projeto base_API com entityFramework não tem referencia porque eu criei do zero.

# Instalação na maquina

- SDK9

- dotnet tool install --global dotnet-ef.

- instala o banco que vai usar, no meu caso foi o postgreSQL;

# Instalção pacote nuget

- no vscode: ctrl+shift+P escreve nuget.

- Instalando entitifremewrok.(versão 9, para funcionar com o pomelo)

dotnet add package Microsoft.EntityFrameworkCore

- Instalando entitifremewrok.tools

- Npgsql.EntityFrameworkCore.PostgreSQL


# Criando o DbContext

- Cria um arquivo chamado AppDbContext na camada de dados.

- Herda a classe DbContext

- Cria o metodo construtor passando as options.

- Cria na mesma classe usando DbSet<Tabela> o banco.

# Criando o controlador

- Cria um arquivo de controler.

- Cria a propriedade "private readonly AppDbContext _dbContext;"

- Cria o metodo construtor, recebendo a injeção de dependencia: "_dbContext  = DbContext;"

# Cria a string de conexao.

- eu botei no segredo na api.

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=BancoPrincipal;Username=postgres;Password=123456;"
  }
}

# Configurando o program para aceitar o contexto do banco

- configurando: 

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<Projeto.Data.Context.DbContext>(options =>
    options.UseNpgsql(connectionString));

# Rodando o migration

- Criar uma Migration:

dotnet ef migrations add InitialCreate --project Projeto.Data --startup-project API_Principal

- Aplicar a Migration no banco (criar tabelas):

dotnet ef database update --project Projeto.Data --startup-project API_Principal

- Listar todas as migrations:

dotnet ef migrations list --project Projeto.Data --startup-project API_Principal

- Remover última migration (se precisar corrigir algo):

dotnet ef migrations remove --project Projeto.Data --startup-project API_Principal

- Reverter para uma migration específica:

dotnet ef database update NomeDaMigration --project Projeto.Data --startup-project API_Principal

# Conceitos basicos sobre mensageria.

 - Producer(Produtor)
 - Exchange
 - Queue(Fila)
 - RabbitqMQ(Broker)
 - Consumer (Consumidor)
 - Subscribe (Assinatura)

# Mapa de como funciona

1) Microservice/Origin(Evento Bus API)

2) publish

3) RabbitMQ(server/Container)

  Pode ter mais de uma fila, fila-A e fila-B
  O Exchange que decide se vai para A ou a B
  
4) subscribe

  O microservice esta escrito em uma fila, e recebe mensagem apenas dela.

5) Microservice A (Event Bus API)

# Tipos de Exchange

 - Direct: Roteia por routing key exata, Logs INFO, ERROR, 
 Manda para uma fila especifica, se quiser logs de erro para uma fila e para outra .


 - Fanout: Envia pra todas as filas, Notificação global, manda para todas as filas, notificação global, se quiser notificar todos os sistema.


 - Topic: Usa padrão(user.*, *.error), Filtros dinâmicos, padrão de chave para distribuir mensagem, mensagem dinamicas, como pedido cancelado ou criado.


 - Headers: Filtra por cabeçalhos, Configurações avançadas, filtra por cabeçalhos customizados.

# Instalação do RabbitMQ

- Baixa o Docker.

- Roda o comando:

docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management


# Criando um produce e consume.

Produce(pedido criado)

Exchange(roteia para fila)

Queeu(fila, quarda a mensagem até processar)

Consume(lê a fila)


# Abra o Visual studio 2026

 - Cria uma solução em branco.

 - Cria 2 projetos console.

 - O primeiro é o Producer.

 - O segundo é o Comsumer. 

 - Instala o pacote nuget, do RabbitMQ.client

 - Cria um projeto de biblioteca de classe para compartilhar o modelo.

 - Cria 2 classes na biblioteca model, item e pedido com as propriedades dela.

# Na classe program no projeto producer, gera o codigo que está no projeto.

 ### 1) Cria as constantes.

 - Cria as constante com os nomes que esta no projeto.

 ### 2) Cria a conection factory, configuração de acesso ao rabbitMQ

  - HostName é localhost porq esta rodando na minha maquina, se fosse produção seria em outro endereço.

  - Port é o padrão de amqp, 

  - CREDENCIAIS de acesso ja vem como padrão, em produção nunca use o guest.

  - Virtualhost é tipo um namespace.

  - AutomaticRecoveryEnabled, bota true, para conectar automaticamente.

  - NetworkRecoveryInterval: defina tanto tempo deve reconectar, definido em 10s.

 ### 3) Cria uma conexão para reutilizar e o canal.

  - Cria canais para modificar de acordo com a demanda.

  - O await using, tanto o canal tanto a conexao seja fechado.

 ### 4) Criando a exchange

  - exchange é um correio central do rabbitmq.

  - recebe as mensagem e decide para qual fila decide enviar.
 
  - Não armazena mensagem, apenas roteia.

  - Parametro:

  - exchange: declara o nome.

  - type: usa uma rootkey, definindo uma chave.

  - durable: true, reinicia do rabittmq.

  - autoDelete: false, significa que a exchange não é deletada se ninguem está usando.

  - argumente: não foi definido. ou bota o valor null.

  ### 5) Declaração da fila

   - declara a fila aonde as mensagem vai ser armazenada.

   - queue: nome da fila

   - durable: true, persistencia da fila, e não das mensagem.

   - exclusive: false, outras conexao pode usar essa fila. 

   - autodelete: false, a fila não é deletada quando o ultimo consumidor consumo.

   - aguments: null, pode passar outras configuração.

### 6) Bind

 - Copa entre exchange e a fila, toda mensagem que chegar na exnchange X, com a routkey X, deve ir para a fila X, 
 o X está definindo nas cosntante.


 - Cria o metodo que chama a fila:

```
    Console.WriteLine("Quantos pedidos você quer enviar?");
    if(!int.TryParse(Console.ReadLine(), out var quantidadePedidos))
    {
        quantidadePedidos = 3;
    }

    for (int i = 0; i < quantidadePedidos; i++)
    {
        var pedido = CriarPedidoFake(i);
        var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(pedido);
        var properties = channel.CreateBasicProperties();

        properties.Persistent = true;
        properties.ContentType = "application/json";

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: body
        );
        Console.WriteLine($"Pedido {pedido.Id} enviado com valor {pedido.ValorTotal}");
      
    }

    static Pedido CriarPedidoFake(int index)
    {
        return new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteEmail = $"cliente{index}@exemplo.com",
            ValorTotal = new Random().Next(100, 5000),
            DataCriacao = DateTime.UtcNow,
            Itens = new List<Item>
                {
                    new Item
                    {
                        NomeProduto = $"Produto {index}-1",
                        Quantidade = new Random().Next(1, 5),
                        PrecoUnitario = new Random().Next(10, 100)
                    }
                }
        };
    }

```

 - Ele Serialize para um array de bites, a mensagem.
 - BasicProperties são metadados.
 - Persistent: a mensagem grava em disco, não se perde
 - ContentType: informa que e json.
 - ContentEncoding: a codificação de caracteres.

 - O BasicPublishAsync: está publicando a mensagem.
 - E indica qual exchange enviar.
 - exchange: qual exchange enviar.
 - routingKey: indica a fila que deve enviar, define o destino.
 - mandatory: false: se não tiver fila, a mensagem e descartada.
 - basicproperties: meta dados configurado.
 - body: dados.

 - debug
  - configura a conexao.
  - cria a conexao.
  - cria a exchange.
  - cria a fila.
  - faz o pedido com dados fake.

 - Testei: e foi para fila do rabbitMQ.

# Consumindo a fila

 - cria a mesma configuração, só copiar e colar toda conexão
 do produce para o consumer.

 - cria uma outra configuração chamada "BasicQosAsync"

 - prefetchSize: tamanho da mensagem

 - prefetchCount: so mandad uma por vez.

 - global: false: limite por canal.

```

await channel.BasicQosAsync(
    prefetchSize: 0,
    prefetchCount: 1,
    global: false    
);

```

- Depois de copiar e criar configurações, instancia o objeto que faz a leitura das filas.

```

var consumer = new RabbitMQ.Client.Events.AsyncEventingBasicConsumer(channel);

```

 - Com isso cria o evento que recebe a mensagem, usando lambda.
 - ele usa o ea.DeliveryTag para identificar cada mensagem.
 - ele Deserialize o body.
 - com isso vc ja tem as informações da mensagem.
 - tem uma linha com delay para simular um processamento.
 - BasicAckAsync: ele confirma que recebeu a mensagem e que pode remover da fila.
 - BasicAckAsync - deliveryTag: informa qual a mensagem estamos confirmando.
 - BasicAckAsync - multiple: false: confirme que recebeu apenas essa mensagem.(util para processamento de bet)


 - Cria um tratamento de erro quando le jsom.
 - Cria um tratamento de erro.

 - BasicNackAsync, nac representa que recebeu mas não consegiu processar.
 - requeue: false, desconsidera caso tenha um deadleri.
 - requeue: true, retorna a mensagem pra fila, e tenta processar novamente.


```
  consumer.ReceivedAsync += async (modal, ea) =>
  {
        try
        {
            var body = ea.Body.ToArray();
            var json = System.Text.Encoding.UTF8.GetString(body);
            var pedido = JsonSerializer.Deserialize<Pedido>(json);

            Console.WriteLine("================================");
            Console.WriteLine($"[Consumer] Pedido recebido: {pedido?.Id}");
            Console.WriteLine($"[Consumer] Cliente: {pedido?.ClienteEmail}");
            Console.WriteLine($"[Consumer] Total: {pedido?.ValorTotal:C}");
            Console.WriteLine($"Cirado: {pedido?.DataCriacao:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine("================================");

            await Task.Delay(2000); // Simula processamento de pedido.

            // Confirma o processamento da mensagem
            await channel.BasicAckAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false
            );

        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Consumer] Erro ao desserializar o pedido: {ex.Message}");
            await channel.BasicNackAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                requeue: true
            );
            throw;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Consumer] Erro ao processar pedido: {ex.Message}");        
            await channel.BasicNackAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                requeue: true
            );
            throw;
        };
  };

```

### Inciando o consumo

 - BasicConsumeAsync: consome depois da configuração.
 - queue : nome da fila
 - autoAck: false: chave de segurança(recomendado), auto não é recomendado
 - consumer: é toda a configuração iniciado do codigo, está na variavel: consumer

 ```

await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: false,
    consumer: consumer
);

```
