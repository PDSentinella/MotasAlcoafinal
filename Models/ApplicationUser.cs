using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public bool Aprovado { get; set; } = false;
}