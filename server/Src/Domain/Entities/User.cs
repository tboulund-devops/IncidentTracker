using Domain.Enums;

namespace Domain.Entities;

public sealed record User
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public RoleType Role { get; set; } = RoleType.User;
    
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(new DateTime(1970, 1, 1));

    public required string Email { get; set; }
    public byte[] PasswordHash { get; set; } = null!;
    public string RefreshTokenHash { get; set; } = string.Empty;
    public DateTime RefreshTokenExpires { get; set; } 
    
    public bool Activated { get; set; } = false;
    public DateTime ExpireDate { get; set; } 
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public static User Create(
        string firstName,
        string lastName,
        string email,
        byte[] passwordHash,
        RoleType role = RoleType.User,
        string phoneNumber = "")
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            PhoneNumber = phoneNumber
        };
    }
}