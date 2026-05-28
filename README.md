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


 


