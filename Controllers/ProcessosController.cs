using ConsultaProcessosApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConsultaProcessosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessosController: ControllerBase
{
    private ConsultaService _consultaService;

    public ProcessosController(ConsultaService consultaService)
    {
        _consultaService =  consultaService;
    }

    /// <summary>
    /// Obter processo atrés do nuemro.
    /// </summary>
    /// <param name="numeroProcesso"></param>
    /// <returns>Retorna o processo que solicitou através do numero dele.</returns>

    [HttpGet("/processos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterProcesso(string numeroProcesso)
    {
        try
        {
            var resultado = await _consultaService.ConsultarEResgistrar(numeroProcesso);
            if (resultado.Count ==0)
            {
                return NotFound("Processo não encontrado.");
            }

            return Ok(resultado);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)// se for uma HttpRequestException e o código recebido for 429, realiza essa execução abaixo
        { 
            return StatusCode(429, "A API do Datajud está sobrecarregada. Tente novamente em alguns minutos.");
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, "Não foi possível consultar a API do Datajud.");
        }
    }
}
