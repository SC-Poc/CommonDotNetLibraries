using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Common.MasterPassword
{
    public interface IPasswordValidationHashRepository
    {
        /// <summary>
        /// clears all validation hashes
        /// </summary>
        /// <returns></returns>
        Task ClearAsync();

        /// <summary>
        /// gets all validation hashes
        /// </summary>
        Task<IEnumerable<string>> GetAsync();

        /// <summary>
        /// inserts batched
        /// </summary>
        Task InsertAsync(IEnumerable<string> validationHashes);
    }
}
