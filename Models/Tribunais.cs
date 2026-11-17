using System.ComponentModel.DataAnnotations;

namespace ConsultaProcessosApi.Models;

public class Tribunais
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Sigla { get; set; }
    [Required]
    public string Nome { get; set; }
    public virtual ICollection<ConsultaProcessos> ConsultaProcessos { get; set; }
}