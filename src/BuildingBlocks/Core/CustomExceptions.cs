namespace BuildingBlocks.Core;

public class CustomException : Exception
{
    public CustomException(string message) : base(message) { }
}

public class ValidationException : CustomException
{
    public ValidationException(string message) : base(message) { }
}

public class NotFoundException : CustomException
{
    public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.") { }
}
