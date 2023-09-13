using CryptoBank.WebAPI.Errors.Exceptions.Base;

namespace WebApi.Common.Errors.Exceptions;

public class InternalErrorException : ErrorException
{
    public InternalErrorException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
