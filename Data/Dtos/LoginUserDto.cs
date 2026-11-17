using System.ComponentModel.DataAnnotations;

namespace ConsultaProcessosApi.Data.Dtos;

public class LoginUserDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória")]
    public string Password { get; set; }
}