using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpGet("qrcode")]
        public IActionResult GetQrCode(double amount)
        {
            string promptPayId = "0610816643";

            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            try
            {
                string payload = GenerateThaiQrPayload(promptPayId, amount);

                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                    using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeImage = qrCode.GetGraphic(20);
                        return File(qrCodeImage, "image/png");
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        private string GenerateThaiQrPayload(string promptPayId, double amount)
        {
            // EMVCo-compliant Thai QR Code Payload Generation

            // Field IDs (FID)
            const string FID_PayloadFormat = "00";
            const string FID_PointOfInitiation = "01";
            const string FID_MerchantInfo = "29"; // For PromptPay
            const string FID_TransactionCurrency = "53";
            const string FID_TransactionAmount = "54";
            const string FID_CountryCode = "58";
            const string FID_CRC = "63";

            // Merchant Info Sub-fields
            const string SubFID_AID = "00";
            const string SubFID_BillerId = "01";

            // Static values
            const string PayloadFormat = "01";
            const string PointOfInitiation = "11"; // 11 = Static QR, 12 = Dynamic QR
            const string AID = "A000000677010111"; // For PromptPay
            const string TransactionCurrency = "764"; // THB
            const string CountryCode = "TH";

            // 1. Payload Format Indicator (FID 00)
            string payload = BuildField(FID_PayloadFormat, "02", PayloadFormat);

            // 2. Point of Initiation Method (FID 01)
            payload += BuildField(FID_PointOfInitiation, "02", PointOfInitiation);

            // 3. Merchant Information (FID 29)
            string billerId = FormatBillerId(promptPayId);
            string merchantInfo = BuildField(SubFID_AID, AID.Length.ToString("D2"), AID);
            merchantInfo += BuildField(SubFID_BillerId, billerId.Length.ToString("D2"), billerId);
            payload += BuildField(FID_MerchantInfo, merchantInfo.Length.ToString("D2"), merchantInfo);

            // 4. Transaction Currency (FID 53)
            payload += BuildField(FID_TransactionCurrency, "03", TransactionCurrency);

            // 5. Transaction Amount (FID 54)
            string amountStr = amount.ToString("F2");
            payload += BuildField(FID_TransactionAmount, amountStr.Length.ToString("D2"), amountStr);

            // 6. Country Code (FID 58)
            payload += BuildField(FID_CountryCode, "02", CountryCode);

            // 7. CRC (FID 63)
            payload += FID_CRC + "04";
            string crc = CalculateCRC16(payload);
            payload += crc;

            return payload;
        }

        private string BuildField(string id, string length, string value)
        {
            return id + length + value;
        }

        private string FormatBillerId(string promptPayId)
        {
            // Remove any non-digit characters
            string cleanedId = new string(promptPayId.Where(char.IsDigit).ToArray());

            if (cleanedId.Length == 10) // Mobile number
            {
                // Format as 0066 + number without leading 0
                return "0066" + cleanedId.Substring(1);
            }
            if (cleanedId.Length == 13) // National ID / Tax ID
            {
                return cleanedId;
            }
            // Add more formats if needed
            throw new ArgumentException("Invalid PromptPay ID format.");
        }

        private string CalculateCRC16(string data)
        {
            ushort crc = 0xFFFF;
            foreach (char c in data)
            {
                crc ^= (ushort)(c << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            return crc.ToString("X4");
        }
    }
}