namespace Rastro.Domain.Exceptions;

public class DuplicateUserEmailException : Exception
{
    public DuplicateUserEmailException(string email) : base ($"El email '{email}' ya esta en uso.")
    {}
    
}