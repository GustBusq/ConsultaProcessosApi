using AutoMapper;
using ConsultaProcessosApi.Data.Dtos;
using ConsultaProcessosApi.Datas;
using ConsultaProcessosApi.Models;
using ConsultaProcessosApi.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultaProcessosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CadastroUsuarioController: ControllerBase
{
    private UsuarioService _manager;


    public CadastroUsuarioController( UsuarioService manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// Cadastra um novo usuário
    /// </summary>
    /// <param name="createUsuarioDto"></param>
    /// <returns>Retorna uma mensagem de sucesso após o cadastro for bem sucedido</returns>
    [HttpPost("cadastro")]
    public async Task<IActionResult> CadastrarUsuario(CreateUsuarioDto createUsuarioDto)
    {
        await _manager.CadastrarUser(createUsuarioDto);
        return Ok("Usuario cadastrado com sucesso!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto loginDto)
    {
        var token = await _manager.LoginUser(loginDto);

        return Ok(token);
    }
}
