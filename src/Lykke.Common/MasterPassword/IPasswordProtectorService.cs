using System.Threading.Tasks;

namespace Lykke.Common.MasterPassword
{
    public interface IPasswordProtectorService
    {
        Task SetPasswordKeyAsync(int partNumber, string partValue);

        Task ValidatePasswordAsync(byte[] validationBytes);

        bool IsPasswordValid { get; }
    }
}
