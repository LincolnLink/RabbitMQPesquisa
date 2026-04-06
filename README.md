# RabbitMQPesquisa

- Desenvolvendo uma API base com Microsoft.EntityFrameworkCore.

- Estudo sobre RabbitMQ , exemplo pratico de como funciona.

- DotnetCore 9. com PostgreeMSQL


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




