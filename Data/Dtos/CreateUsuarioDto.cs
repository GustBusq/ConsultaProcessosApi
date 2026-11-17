using System.ComponentModel.DataAnnotations;

namespace ConsultaProcessosApi.Data.Dtos;

public class CreateUsuarioDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório"), 
        MaxLength(100, ErrorMessage = "O nome de usuário não pode exceder 100 caracteres")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "A data de nascimento é obrigatória")]
    public DateTime DataNascimento { get; set; }

    [Required(ErrorMessage = "O email é obrigatório")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
    [Compare("Password", ErrorMessage = "As senhas não coincidem")]
    public string ConfirmPassword { get; set; }
}
