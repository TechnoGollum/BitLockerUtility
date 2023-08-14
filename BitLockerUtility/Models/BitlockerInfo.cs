using System.Collections.Generic;

namespace BitLockerUtility.Models
{
    internal class BitlockerInfo
    {
        public string? Size { get; set; }
        public string? BitLockerVersion { get; set; }
        public string? ConversionStatus { get; set; }
        public string? PercentageEncrypted { get; set; }
        public string? EncryptionMethod { get; set; }
        public string? ProtectionStatus { get; set; }
        public string? LockStatus { get; set; }
        public string? IdentificationField { get; set; }
        public string? AutomaticUnlock { get; set; }
        public List<string>? KeyProtectors { get; set; }
    }
}
