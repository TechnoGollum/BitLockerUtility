using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using CmdBitlockerService;
namespace BitLockerUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HidePasswordFieds();
            HidePinFieds();
            HideUnlockFields();
        }
        List<Dictionary<string, string>> volimeListEncrypted = new List<Dictionary<string, string>>();
        string? selectedVolume = null;
        readonly IBitlockerService bitlockerService = new BitlockerService();

        private void ShowUnlockFields()
        {
            UnlockPassword.Visibility = Visibility.Visible;
            UnlockPasswordBtn.Visibility = Visibility.Visible;
            UnlockWithPassword.Visibility = Visibility.Visible;
        }
        private void HideUnlockFields()
        {
            UnlockPassword.Visibility = Visibility.Hidden;
            UnlockPasswordBtn.Visibility = Visibility.Hidden;
            UnlockWithPassword.Visibility = Visibility.Hidden;
        }
        private void HidePasswordFieds()
        {
            SavePasswordChange.Visibility = Visibility.Hidden;
            PasswordLabel.Visibility = Visibility.Hidden;
            PasswordTextBox.Visibility = Visibility.Hidden;
            ConfirmPasswordLabel.Visibility = Visibility.Hidden;
            ConfirmPasswordTextBox.Visibility = Visibility.Hidden;
        }
        private void ShowPasswordFields()
        {
            SavePasswordChange.Visibility = Visibility.Visible;
            PasswordLabel.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Visible;
            ConfirmPasswordLabel.Visibility = Visibility.Visible;
            ConfirmPasswordTextBox.Visibility = Visibility.Visible;
        }
        private void HidePinFieds()
        {
            SavePinChange.Visibility = Visibility.Hidden;
            PinLabel.Visibility = Visibility.Hidden;
            PinTextBox.Visibility = Visibility.Hidden;
            ConfirmPinLabel.Visibility = Visibility.Hidden;
            ConfirmPinTextBox.Visibility = Visibility.Hidden;
        }
        private void ShowPinFields()
        {
            SavePinChange.Visibility = Visibility.Visible;
            PinLabel.Visibility = Visibility.Visible;
            PinTextBox.Visibility = Visibility.Visible;
            ConfirmPinLabel.Visibility = Visibility.Visible;
            ConfirmPinTextBox.Visibility = Visibility.Visible;
        }

        private List<Dictionary<string, string>> ListVolumesToModel(string[] volumesList)
        {
            //TODO: Use Model
            //BitlockerInfo bitLockerInfo = new BitlockerInfo(); 
            //bitLockerInfo.KeyProtectors = new List<string>();

            volimeListEncrypted = new List<Dictionary<string, string>>();
            foreach (var volume in volumesList.Skip(1))
            {
                // Using Regex to extract key-value pairs
                var keyValuePairs = Regex.Matches(volume, @"(\w+(?:\s+\w+)*)\s*:\s*([^\r\n]+)");
                Dictionary<string, string> keyValuePairs1 = new Dictionary<string, string>
                {
                    { "Volume", volume.Split("\n")[0].Split(':')[0] }
                };
                foreach (Match match in keyValuePairs)
                {
                    string line = match.Value.Trim();
                    string[] parts = line.Split(':');

                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        keyValuePairs1.Add(key, value);
                    }
                }
                volimeListEncrypted.Add(keyValuePairs1);
            }
            return volimeListEncrypted;
        }

        private void FillDetails(int selectedIndex)
        {
            var selectedDisk = volimeListEncrypted[selectedIndex];
            selectedVolume = selectedDisk["Volume"];
            if (selectedDisk != null)
            {
                BitlockerVolumeTextBox.Text = selectedVolume;
                BitlockerVersionTextBox.Text = selectedDisk["BitLocker Version"];
                BitlockerLockedStatusTextBox.Text = selectedDisk["Lock Status"];
                BitlockerProtectorsTextBox.Text = selectedDisk["Key Protectors"];

                if (Regex.IsMatch(selectedDisk["Key Protectors"].Replace(" ", ""), "Numerical") && selectedDisk["Lock Status"] == "Unlocked")
                {
                    ChangePin.Visibility = Visibility.Collapsed;
                    ChangePassword.IsEnabled = true;
                }
                else { ChangePassword.IsEnabled = false; }
                if (Regex.IsMatch(selectedDisk["Key Protectors"], "PIN") && selectedDisk["Lock Status"] == "Unlocked")
                {
                    ChangePassword.Visibility = Visibility.Collapsed;
                    ChangePin.IsEnabled = true;
                }
                else { ChangePin.IsEnabled = false; }
                if (selectedDisk["Lock Status"] == "Locked" && Regex.IsMatch(selectedDisk["Key Protectors"].Replace(" ", ""), "Numerical"))
                {
                    LockVolumeBtn.IsEnabled = false;
                    ShowUnlockFields();
                }
                else if (selectedDisk["Lock Status"] == "Unlocked" && Regex.IsMatch(selectedDisk["Key Protectors"].Replace(" ", ""), "Numerical"))
                {
                    LockVolumeBtn.IsEnabled = true;
                    HideUnlockFields();
                }
                else
                {
                    LockVolumeBtn.IsEnabled = false;
                    HideUnlockFields();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var volumesList = ListVolumesToModel(bitlockerService.ListVolume());
            ListOfVolumes.Items.Clear();
            foreach (var volume in volumesList)
            {
                ListOfVolumes.Items.Add("Volume: " + volume["Volume"]);
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string passwordFromTextbox = PasswordTextBox.Password.ToString();
            string confirmPasswordFromTextbox = ConfirmPasswordTextBox.Password.ToString();
            string response = bitlockerService.ChangeBitlockerPassword(passwordFromTextbox, confirmPasswordFromTextbox, selectedVolume);
            System.Windows.MessageBox.Show(response);
        }

        private void ChangePassword_Click_1(object sender, RoutedEventArgs e)
        {
            ShowPasswordFields();
            HidePinFieds();
        }

        private void ChangePin_Click(object sender, RoutedEventArgs e)
        {
            ShowPinFields();
            HidePasswordFieds();
        }

        private void SavePinChange_Click(object sender, RoutedEventArgs e)
        {
            string pinFromTextbox = PinTextBox.Password.ToString();
            string confirmPinFromTextbox = ConfirmPinTextBox.Password.ToString();
            string response = bitlockerService.ChangeBitlockerPin(pinFromTextbox, confirmPinFromTextbox, selectedVolume);
            System.Windows.MessageBox.Show(response);
        }

        private void UnlockPassword1_Click(object sender, RoutedEventArgs e)
        {
            var response = bitlockerService.UnlockVolume(UnlockPassword.Password, selectedVolume);
            System.Windows.MessageBox.Show(response);
            var selectedIndex = ListOfVolumes.SelectedIndex;
            var volumesList = ListVolumesToModel(bitlockerService.ListVolume());
            ListOfVolumes.Items.Clear();
            foreach (var volume in volumesList)
            {
                ListOfVolumes.Items.Add("Volume: " + volume["Volume"]);
            }
            FillDetails(selectedIndex);
            ListOfVolumes.SelectedIndex = selectedIndex;
        }

        private void LockVolume_Click(object sender, RoutedEventArgs e)
        {
            var response = bitlockerService.LockVolume(selectedVolume);
            var selectedIndex = ListOfVolumes.SelectedIndex;
            System.Windows.MessageBox.Show(response);
            var volumesList = ListVolumesToModel(bitlockerService.ListVolume());
            ListOfVolumes.Items.Clear();
            foreach (var volume in volumesList)
            {
                ListOfVolumes.Items.Add("Volume: " + volume["Volume"]);
            }
            FillDetails(selectedIndex);
            ListOfVolumes.SelectedIndex = selectedIndex;
        }

        private void ListDisk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ListBox listbox = (System.Windows.Controls.ListBox)sender;
            if (listbox.SelectedIndex >= 0)
            {
                FillDetails(listbox.SelectedIndex);
            }
        }
    }

}

