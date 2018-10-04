using System;

namespace Lykke.Common.MasterPassword.Exceptions
{
    /// <summary>
    /// Indicates that password description failed
    /// </summary>
    public class MasterPasswordDecryptionException : MasterPasswordExceptionBase
    {
        public MasterPasswordDecryptionException(string message, Exception innerException):base(message, innerException)
        {
        }
    }
}
