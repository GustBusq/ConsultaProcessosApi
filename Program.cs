using ConsultaProcessosApi.Datas;
using ConsultaProcessosApi.Models;
using ConsultaProcessosApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);


// ------ METADE 1: REGISTRAR SERVIÇOS (antes do Build) ------

// 1. Controllers + Swagger — o básico de toda API - HttpClient
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// 2. Banco de dados — o EF Core precisa saber ONDE e COMO conectar
var connString = builder.Configuration["ConnectionStrings:UsuarioConnection"];

builder.Services.AddControllers();

// Criamos o contexto do banco de dados e configuramos a conexão com o MySQL
builder.Services.AddDbContext<ConsultaContext>(opts=>
    opts.UseLazyLoadingProxies().
        UseMySql(connString, ServerVersion.AutoDetect(connString))
);


// 3. Identity — PRECISA vir depois do AddDbContext (vai utilizar o mesmo contexto)

builder.Services.AddIdentity<Usuario, IdentityRole>(). // Ativa o Identity com a classe Usuario e a classe de perfil (Role)
    AddEntityFrameworkStores<ConsultaContext>() // Falamos qual DbContext irar armazenar as informações do Identity
    .AddDefaultTokenProviders(); // Adiciona os provedores de token padrão (para reset de senha, confirmação de email, etc.)


// 4. Autenticação via JWT — como a API vai VALIDAR o token recebido
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["SymmetricSecurityKey"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
}
);

// 5. AutoMapper — escaneia o projeto procurando todas as classes "...Profile"
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 6. Seus próprios Services — cada um que você criou precisa ser registrado
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddHttpClient<ConsultaService>(
    client =>
        {
            client.BaseAddress = new Uri("https://api-publica.datajud.cnj.jus.br/");
            client.DefaultRequestHeaders.Add("Authorization",
                $"APIKey {builder.Configuration["AuthorizationKey"]}");
        });


// ----- METADE 2: PIPELINE HTTP (ordem IMPORTA de verdade) -----

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();