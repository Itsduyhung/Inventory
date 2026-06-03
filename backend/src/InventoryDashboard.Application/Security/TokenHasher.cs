using System.Security.Cryptography;
using System.Text;

namespace InventoryDashboard.Application.Security;

public static class TokenHasher
{
    public static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
