using Microsoft.AspNetCore.Identity;

/// <summary>
/// Representa um utilizador da aplicação, estendendo o IdentityUser para incluir propriedades personalizadas.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Indica se o utilizador está aprovado para aceder à aplicação.
    /// </summary>
    public bool Aprovado { get; set; } = false;
}