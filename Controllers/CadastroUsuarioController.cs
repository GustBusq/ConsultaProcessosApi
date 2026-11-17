using AutoMapper;
using ConsultaProcessosApi.Data.Dtos;
using ConsultaProcessosApi.Datas;
using ConsultaProcessosApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaProcessosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CadastroUsuarioController: ControllerBase
{
    private UsuarioService _context;

    public CadastroUsuarioController( UsuarioService context)
    {
        _context = context;
    }

    /// <summary>
    /// Cadastra um novo usuário
    /// </summary>
    /// <param name="createUsuarioDto"></param>
    /// <returns>Retorna uma mensagem de sucesso após o cadastro for bem sucedido</returns>
    [HttpPost]
    public async Task<IActionResult> CadastrarUsuario(CreateUsuarioDto createUsuarioDto)
    {
        await _context.CadastrarUser(createUsuarioDto);
        return Ok("Usuario cadastrado com sucesso!");
    }
}
