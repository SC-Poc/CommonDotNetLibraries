using Lykke.Common.MasterPassword.Exceptions;

namespace Lykke.Common.MasterPassword
{
    /// <summary>
    /// Indicates that you need to set password parts before proceed operation
    /// </summary>
    public class MasterPasswordNotInitiatedException: MasterPasswordExceptionBase
    {
        public MasterPasswordNotInitiatedException(string message):base(message)
        {
        }
    }
}
