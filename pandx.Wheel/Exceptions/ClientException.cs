namespace pandx.Wheel.Exceptions;

public class ClientException : Exception, IHasErrorCode
{
    public ClientException()
    {
    }

    public ClientException(string message) : base(message)
    {
    }

    public ClientException(string message, int errorCode) : this(message)
    {
        ErrorCode = errorCode;
    }

    public ClientException(string message, string details) : this(message)
    {
        Details = details;
    }

    public ClientException(string message, string details, int errorCode) : this(message, errorCode)
    {
        Details = details;
    }

    public ClientException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ClientException(string message, int errorCode, Exception innerException) : this(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public ClientException(string message, string details, Exception innerException) : this(message, innerException)
    {
        Details = details;
    }

    public ClientException(string message, string details, int errorCode, Exception innerException) : this(message,
        details,
        innerException)
    {
        ErrorCode = errorCode;
    }

    public string Details { get; set; } = default!;

    public int ErrorCode { get; set; }
}