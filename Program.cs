using ConsultaProcessosApi.Datas;
using ConsultaProcessosApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);


// ------ METADE 1: REGISTRAR SERVIÇOS (antes do Build) ------

// 1. Controllers + Swagger — o básico de toda API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Banco de dados — o EF Core precisa saber ONDE e COMO conectar
var connString = builder.Configuration.GetConnectionString(builder.Configuration["ConnectionStrings:UsuarioConnection"]);

builder.Services.AddControllers();

// Criamos o contexto do banco de dados e configuramos a conexão com o MySQL
builder.Services.AddDbContext<ConsultaContext>(opts=>
    opts.
        UseMySql(connString, ServerVersion.AutoDetect(connString))
);


// 3. Identity — PRECISA vir depois do AddDbContext (vai utilizar o mesmo contexto)

builder.Services.AddIdentity<Usuario, IdentityRole>(). // Ativa o Identity com a classe Usuario e a classe de perfil (Role)
    AddEntityFrameworkStores<ConsultaContext>() // Falamos qual DbContext irar armazenar as informações do Identity
    .AddDefaultTokenProviders(); // Adiciona os provedores de token padrão (para reset de senha, confirmação de email, etc.)

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
