namespace TNI.Api.CleanArchitecture.Application.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Access is forbidden.") { }
}
