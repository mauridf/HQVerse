namespace HQVerse.Domain.Exceptions;

public class ValidationException : DomainException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Validation failed. See Errors for details.")
    {
        Errors = errors;
    }
}