using Rastro.Application.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Rastro.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
        => BCryptNet.HashPassword(password);

    public bool VerifyPassword(string password, string hashedPassword) => BCryptNet.Verify(password, hashedPassword);
}