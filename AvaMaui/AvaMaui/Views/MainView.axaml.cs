using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AvaMaui.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            btn_gps.Click += Btn_gps_Click;
            btn_battery.Click += Btn_battery_Click;
            btn_cappic.Click += Btn_cappic_Click;
            btn_pickpic.Click += Btn_pickpic_Click;
            // btn_fly.Click += Btn_fly_Click;
        }

        private void Btn_fly_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var ctl = sender as Control;
            if (ctl != null)
            {
                FlyoutBase.ShowAttachedFlyout(ctl);
            }
        }

        private async void Btn_pickpic_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            FileResult? photo = await MediaPicker.Default.PickVideoAsync();
            if (photo != null)
            {
                string path = photo.FullPath;
                bool isexists = false;
                if (File.Exists(path))
                {
                    isexists = true;
                }
                txt_block.Text = path + isexists.ToString();
            }
        }

        private async void Btn_cappic_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {

                    // save the file into local storage
                    //FileSystem.CacheDirectory
                    string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                    txt_block.Text = localFilePath;
                    using Stream sourceStream = await photo.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(localFilePath);

                    await sourceStream.CopyToAsync(localFileStream);
                }
            }
        }

        private void Btn_battery_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            txt_block.Text = (Battery.Default.ChargeLevel * 100).ToString();
        }

        private async void Btn_gps_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (OperatingSystem.IsAndroid())
            {
                try
                {
                    var location = await Geolocation.GetLocationAsync();
                    txt_block.Text = location.Longitude.ToString() + "|" + location.Longitude.ToString();
                }
                catch (Exception ex)
                {
                    txt_block.Text = ex.ToString();
                }

            }
        }
    }
}