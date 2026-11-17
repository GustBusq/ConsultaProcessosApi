using AutoMapper;
using ConsultaProcessosApi.Data.Dtos;
using ConsultaProcessosApi.Models;
using Microsoft.AspNetCore.Identity;

namespace ConsultaProcessosApi.Service;

public class UsuarioService
{
    private IMapper _mapper;
    private UserManager<Usuario> _userManager;
    private SignInManager<Usuario> _signInManager;

    public UsuarioService(IMapper mapper, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task CadastrarUser(CreateUsuarioDto createUsuarioDto)
    {
        // 1º Mapeiamos o DTO para a entidade Usuario
        var usuario = _mapper.Map<CreateUsuarioDto, Usuario>(createUsuarioDto);
        // 2º Chamamos o método CreateAsync do UserManager para criar o usuário no banco de dados
        var result = await _userManager.CreateAsync(usuario, createUsuarioDto.Password);
        // 3º Validamos se a criação do usuário foi bem-sucedida
        if (!result.Succeeded)
        {
            throw new ApplicationException("Falha ao cadastrar usuário: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task LoginUser(LoginUserDto loginUserDto)
    {
        var result = await _signInManager.PasswordSignInAsync(loginUserDto.UserName, loginUserDto.Password, false, false);
        if (!result.Succeeded)
        {
            throw new ApplicationException("Falha ao realizar login: Usuário ou senha inválidos.");
        }
    }
}
