using QRCoder;
using PosSystem.Helpers;
using Microsoft.AspNetCore.Components;

namespace PosSystem.Services
{
    public class QrCoreService
    {
        private readonly NavigationManager _nav;

        public QrCoreService(NavigationManager nav) => _nav = nav;

        // 1. Core logic to create the Encrypted URL for scanning
        public string CreateSecureUrl(string type, string id)
        {
            // Format: Type|ID|Timestamp (to prevent replay attacks)
            string rawData = $"{type}|{id}|{DateTime.UtcNow.Ticks}";
            string encryptedData = EncryptionHelper.Encrypt(rawData);
            return $"{_nav.BaseUri}verify?data={Uri.EscapeDataString(encryptedData)}";
        }

        // 2. Generate the actual QR Code Image as Base64
        public string GenerateQrImage(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeAsPngByteArr)}";
        }
        public (string Type, string Id, bool IsValid) VerifyQrData(string encryptedData)
        {
            try
            {
                string decrypted = EncryptionHelper.Decrypt(encryptedData);
                string[] parts = decrypted.Split('|');

                if (parts.Length < 3) return (null, null, false);

                string type = parts[0];
                string id = parts[1];
                long ticks = long.Parse(parts[2]);

                // Optional: Check if the QR is too old (e.g., > 24 hours for security)
                var createdDate = new DateTime(ticks);
                if (DateTime.UtcNow > createdDate.AddDays(1) && type == "Ticket")
                    return (type, id, false); // Expired

                return (type, id, true);
            }
            catch
            {
                return (null, null, false);
            }
        }
    }
}