# RabbitMQPesquisa

- Desenvolvendo uma API base com Microsoft.EntityFrameworkCore.

- Estudo sobre RabbitMQ , exemplo pratico de como funciona.

- DotnetCore 9.

# Instalção

- no vscode: ctrl+shift+P escreve nuget.

- Instalando entitifremewrok.(versão 9, para funcionar com o pomelo)

dotnet add package Microsoft.EntityFrameworkCore

- Instalando entitifremewrok.tools

- Pomelo.EntityFrameworkCore.MySql


# Criando o DbContext

- Cria um arquivo chamado AppDbContext na camada de dados.

- Herda a classe DbContext

- Cria o metodo construtor passando as options.

- Cria na mesma classe usando DbSet<Tabela> o banco.

# Criando o controlador

- Cria um arquivo de controler.

- Cria a propriedade "private readonly PersonagemDbContext _dbContext;"

- Cria o metodo construtor, recebendo a injeção de dependencia: "_dbContext  = personagemDbContext;"

# Cria a string de conexao.

- eu botei no segredo na api.

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BancoPrincipal;User=root;Password=123456;"
  }
}

# Configurando o program para aceitar o contexto do banco

- configurando: 

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 40));

builder.Services.AddDbContext<PersonagemDbContext>(options =>
           options.UseMySql(connectionString, serverVersion));

# Rodando o migration

dotnet ef migrations add InitialCreate --project Projeto.Data --startup-project API_Principal

dotnet ef database update --project Projeto.Data --startup-project API_Principal




