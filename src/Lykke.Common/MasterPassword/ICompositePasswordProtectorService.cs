using System;
using System.Threading.Tasks;

namespace Lykke.Common.MasterPassword
{
    /// <summary>
    /// Protects password with 3 keys encryption algrorithm
    /// </summary>
    public interface ICompositePasswordProtectorService
    {
        /// <summary>
        /// Set one of the password parts 
        /// </summary>
        /// <param name="partNumber">password part number. Possible values 1,2,3 </param>
        /// <param name="partValue">password part number value</param>
        Task SetPasswordPartAsync(int partNumber, string partValue);

        /// <summary>
        /// Is password successfully initiated
        /// </summary>
        bool IsPasswordValid { get; }
        
        /// <summary>
        /// Encrpyts input value
        /// </summary>
        /// <param name="input">value to encrpypt</param>
        string Ecrypt(string input);

        /// <summary>
        /// Encrpyts input value
        /// </summary>
        /// <param name="input">value to dncrpypt</param>
        string Decrypt(string input);

        /// <summary>
        /// Get password if it is initiated
        /// </summary>
        string GetFullPassword();
    }
}
