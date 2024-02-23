using System;

namespace Movies.Client.Exceptions
{
    public class UnAuthorizeException : AppException
    {
        public UnAuthorizeException()
           : base(ApiResultStatusCode.LogicError)
        {
        }

        public UnAuthorizeException(string message)
            : base(ApiResultStatusCode.LogicError, message)
        {
        }

        public UnAuthorizeException(object additionalData)
            : base(ApiResultStatusCode.LogicError, additionalData)
        {
        }

        public UnAuthorizeException(string message, object additionalData)
            : base(ApiResultStatusCode.LogicError, message, additionalData)
        {
        }

        public UnAuthorizeException(string message, Exception exception)
            : base(ApiResultStatusCode.LogicError, message, exception)
        {
        }

        public UnAuthorizeException(string message, Exception exception, object additionalData)
            : base(ApiResultStatusCode.LogicError, message, exception, additionalData)
        {
        }
    }
}
