namespace pandx.Wheel.Exceptions;

public class WheelException : Exception, IHasErrorCode
{
    public WheelException()
    {
    }

    public WheelException(string message) : base(message)
    {
    }

    public WheelException(string message, int errorCode) : this(message)
    {
        ErrorCode = errorCode;
    }

    public WheelException(string message, string details) : this(message)
    {
        Details = details;
    }

    public WheelException(string message, string details, int errorCode) : this(message, errorCode)
    {
        Details = details;
    }

    public WheelException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public WheelException(string message, int errorCode, Exception innerException) : this(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public WheelException(string message, string details, Exception innerException) : this(message, innerException)
    {
        Details = details;
    }

    public WheelException(string message, string details, int errorCode, Exception innerException) : this(message,
        details,
        innerException)
    {
        ErrorCode = errorCode;
    }

    public string Details { get; set; } = default!;
    public int ErrorCode { get; set; }
}