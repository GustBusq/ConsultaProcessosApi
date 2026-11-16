using ConsultaProcessosApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConsultaProcessosApi.Datas;

public class ConsultaContext : IdentityDbContext<Usuario>
{
    public ConsultaContext(DbContextOptions<ConsultaContext> options)
        : base(options)
    {}

    public DbSet<ConsultaProcessos> Consultas { get; set; }
    public DbSet<Tribunais> Tribunais { get; set; }
}
