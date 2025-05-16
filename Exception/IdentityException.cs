using Microsoft.AspNetCore.Identity;

public class IdentityException : Exception
{
    public IEnumerable<IdentityError> Errors { get; init; }

    public IdentityException(IdentityResult result)
    {
        this.Errors = result.Errors;
    }
}