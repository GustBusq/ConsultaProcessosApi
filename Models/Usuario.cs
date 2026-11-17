using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConsultaProcessosApi.Models;

public class Usuario: IdentityUser
{
    public Usuario(): base()
    {
    }
    public DateTime DataCadastro { get; set; } = DateTime.Now;
}