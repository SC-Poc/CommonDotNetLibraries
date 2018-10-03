using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Common.MasterPassword
{
    public interface IPasswordValidationHashRepository
    {
        Task ClearAsync();

        Task<IEnumerable<string>> GetAsync();

        Task InsertAsync(IEnumerable<string> validationHashes);
    }
}
