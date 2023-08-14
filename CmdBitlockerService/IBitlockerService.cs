namespace CmdBitlockerService
{
    public interface IBitlockerService
    {
        public string[]? ListVolume();
        public string? UnlockVolume(string password, string selectedVolume);
        public string? LockVolume(string selectedVolume);
        public string ChangeBitlockerPassword(string password, string confirmPassword, string selectedVolume);
        public string ChangeBitlockerPin(string pin, string confirmPin, string selectedVolume);
    }
}
