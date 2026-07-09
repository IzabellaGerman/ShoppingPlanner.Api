using System.ComponentModel.DataAnnotations;

namespace ShoppingPlanner.Api.Dtos;

public class RegisterDto
    {
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
    }

public class LoginDto
    {
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
    }

public class AuthResponseDto
    {
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    }