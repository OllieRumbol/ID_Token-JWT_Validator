namespace JWTValidatorService;

public class BuildingException : Exception
{
    public BuildingException()
    {
    }

    public BuildingException(String message)
        : base(message)
    {
    }

    public BuildingException(String message, Exception inner)
        : base(message, inner)
    {
    }
}