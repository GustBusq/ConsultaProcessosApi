using System.ComponentModel.DataAnnotations;

namespace ConsultaProcessosApi.Models;

public class ConsultaProcessos
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string NumeroProcesso { get; set; }
    [Required]
    public string ClasseProcesso { get; set; }
    [Required]
    public string AssuntoProcesso { get; set; }
    public string DataConsulta { get; set; } =DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

    public int TribunaisId { get; set; }
    public virtual Tribunais Tribunais { get; set; }

    [Required]
    public int UsuarioId { get; set; } = 1;
}
