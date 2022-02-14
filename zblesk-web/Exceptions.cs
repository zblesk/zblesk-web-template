namespace zblesk_web;

public abstract class zblesk_webException : Exception
{
    public virtual int StatusCode { get; set; } = 500;

    public zblesk_webException() : base()
    {
    }

    public zblesk_webException(string message) : base(message)
    {
    }

    public zblesk_webException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class PermissionException : zblesk_webException
{
    public PermissionException() : base()
    {
    }

    public PermissionException(string message) : base(message)
    {
    }

    public PermissionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class ForbiddenException : zblesk_webException
{
    public ForbiddenException() : base()
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class DatastoreException : zblesk_webException
{
    public DatastoreException() : base()
    {
    }

    public DatastoreException(string message) : base(message)
    {
    }

    public DatastoreException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class ValidationException : zblesk_webException
{
    public override int StatusCode { get; set; } = 400;

    public ValidationException() : base()
    {
    }

    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
