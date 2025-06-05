using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
            btn_writefile.Click += Btn_writefile_Click;
            btn_readfile.Click += Btn_readfile_Click;
            btn_pickfileread.Click += Btn_pickfileread_Click;
        }

        private async void Btn_pickfileread_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IStorageProvider isp = TopLevel.GetTopLevel(this).StorageProvider;
            var files = await isp.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                AllowMultiple = false,
                Title = "选择文件",
                FileTypeFilter = new[] { new Avalonia.Platform.Storage.FilePickerFileType("文件") { Patterns = new[] { "*" } } }
            });
            if (files.Count() > 0)
            {
                StringBuilder sb = new StringBuilder();
                // string filepath = files[0].TryGetLocalPath();
                using Stream fs = await files[0].OpenReadAsync();//非专属目录下：最好是直接通过 流，而不是TryGetLocalPath
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    char[] buffer = new char[1024];
                    int bytesRead;

                    while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        sb.Append(buffer, 0, bytesRead);
                    }
                }
                FileContent.Text = sb.ToString();
            }
        }

        private async void Btn_readfile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IStorageProvider isp = TopLevel.GetTopLevel(this).StorageProvider;
            IStorageFolder isf = await isp.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);
            string localpath = isf.TryGetLocalPath();
            string filepath = Path.Combine(localpath, "data.txt");
            StringBuilder sb = new StringBuilder();
            int bufferSize = 1024;
            if (File.Exists(filepath))
            {
                using FileStream fs = new FileStream(filepath, FileMode.Open);
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    char[] buffer = new char[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        sb.Append(buffer, 0, bytesRead);
                    }
                }
                FileContent.Text = sb.ToString();
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async void Btn_writefile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IStorageProvider isp = TopLevel.GetTopLevel(this).StorageProvider;
            IStorageFolder isf = await isp.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);
            string localpath = isf.TryGetLocalPath();
            string filepath = Path.Combine(localpath, "data.txt");
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            using FileStream fs = new FileStream(filepath, FileMode.Create);
            byte[] buffer = Encoding.UTF8.GetBytes(FileContent.Text);
            await fs.WriteAsync(buffer, 0, buffer.Count());
            await fs.FlushAsync();
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
                    var storageprovider = TopLevel.GetTopLevel(this)?.StorageProvider;
                    IStorageFolder folder = await storageprovider.TryGetWellKnownFolderAsync(Avalonia.Platform.Storage.WellKnownFolder.Downloads);
                    string localFilePath = Path.Combine(folder.TryGetLocalPath(), photo.FileName);
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