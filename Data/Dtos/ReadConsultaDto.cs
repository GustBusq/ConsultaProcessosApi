using ConsultaProcessosApi.Models;
using System.ComponentModel.DataAnnotations;

namespace ConsultaProcessosApi.Data.Dtos;

public class ReadConsultaDto
{
    public int Id { get; set; }
    public string NumeroProcesso { get; set; }
    public string ClasseProcesso { get; set; }
    public string AssuntoProcesso { get; set; }
    public string DataConsulta { get; set; }
    public string Tribunal { get; set; }
}
