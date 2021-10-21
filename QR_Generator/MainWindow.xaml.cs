using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing;
using ZXing.QrCode;

namespace QR_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QrCodeEncodingOptions qrOptions = new QrCodeEncodingOptions
        {
            DisableECI = true,
            CharacterSet = "UTF-8",
            Width = 400,
            Height = 400,
        };

        Bitmap result;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if(result==null)
            {
                MessageBox.Show("Image is null", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string filename = "";
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFile.Title = "Save QR Code Image";
            saveFile.DefaultExt = "bmp";
            saveFile.Filter = "Bitmap files(*.bmp)|*.bmp";
            if (saveFile.ShowDialog() == DialogResult.HasValue || saveFile.FileName != "")
            {
                filename = saveFile.FileName.ToString();
                result.Save(filename);
            }
        }

        private void BtnErase_Click(object sender, RoutedEventArgs e)
        {
            this.TboxData.Clear();
            this.QrImage.Source = null;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        private void TboxData_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.TboxData.Text) || String.IsNullOrEmpty(this.TboxData.Text))
            {
                this.LblLength.Content = this.TboxData.Text.Length.ToString() + " bytes";
                this.QrImage.Source = null;
            }
            else
            {
                this.LblLength.Content = this.TboxData.Text.Length.ToString() + " bytes";
                var qr = new ZXing.BarcodeWriter();
                qr.Options = qrOptions;
                qr.Format = ZXing.BarcodeFormat.QR_CODE;
                try
                {
                    result = new Bitmap(qr.Write(this.TboxData.Text.Trim()));
                    this.QrImage.Source = ImageSourceFromBitmap(result);
                }
                catch
                {
                    MessageBox.Show("Invalid data! Length: " + this.TboxData.Text.Length.ToString(), "QR Generate Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
