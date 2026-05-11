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


- 

