using System;

namespace Lykke.Common.MasterPassword.Exceptions
{
    public abstract class MasterPasswordExceptionBase:Exception
    {
        protected MasterPasswordExceptionBase(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
