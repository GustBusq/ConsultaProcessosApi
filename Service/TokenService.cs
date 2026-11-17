using ConsultaProcessosApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ConsultaProcessosApi.Service;

public class TokenService
{
    private IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Usuario usuario)
    {
        Claim[] claims = new[]
        {
            new Claim("username",usuario.UserName ), 
            new Claim("id", usuario.Id),
            new Claim("LoginTimestamp", DateTime.UtcNow.ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SymmetricSecurityKey"])); // Chave simétrica para assinar o token

        var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256); // assinatura do token

        var token = new JwtSecurityToken
            (
                expires: DateTime.UtcNow.AddHours(1), // tempo que irá expirar o token
                claims: claims, // dados usados para gerar o token
                signingCredentials: signingCredentials // assinatura do token
            ); 

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
