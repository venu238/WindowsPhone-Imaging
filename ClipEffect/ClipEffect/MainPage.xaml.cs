using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ClipEffect.Resources;
using System.Windows.Media.Imaging;
using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;

namespace ClipEffect
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var mask = new WriteableBitmap(First, null);
            var image = new WriteableBitmap(Second, null);

            var bmp = new WriteableBitmap((int)Second.ActualWidth, (int)Second.ActualHeight);

            using (BitmapImageSource imageSource = new BitmapImageSource(image.AsBitmap()))
            using (BitmapImageSource maskSource = new BitmapImageSource(mask.AsBitmap()))
            using (var effect = new ClipnCrop(imageSource, maskSource))
            using (var renderer = new WriteableBitmapRenderer(effect, bmp, OutputOption.PreserveAspectRatio))
            {
                await renderer.RenderAsync();

                Final.Source = bmp;
            }

            bmp = null;
        }

        private async void ClipnCrop_Click(object sender, RoutedEventArgs e)
        {
            var mask = new WriteableBitmap(First, null);
            var image = new WriteableBitmap(Second, null);

            var bmp = new Bitmap(new Windows.Foundation.Size((int)Second.ActualWidth, (int)Second.ActualHeight), ColorMode.Ayuv4444);

            Windows.Foundation.Size outputSize;
            Windows.Foundation.Point corner;


            using (BitmapImageSource imageSource = new BitmapImageSource(image.AsBitmap()))
            using (BitmapImageSource maskSource = new BitmapImageSource(mask.AsBitmap()))
            using (var effect = new ClipnCrop(imageSource, maskSource))
            using (var renderer = new BitmapRenderer(effect, bmp, OutputOption.PreserveAspectRatio))
            {
                await renderer.RenderAsync();

                outputSize = effect.CropSize;
                corner = effect.Corner;

                // cropping image

                var reframeFilter = new ReframingFilter(new Windows.Foundation.Rect(corner, outputSize), 0.0);
                var cropdWbm = new WriteableBitmap((int)outputSize.Width, (int)outputSize.Height);

                using (var source1 = new BitmapImageSource(bmp))
                using (var cropFilter = new FilterEffect(source1))
                using (var renderer1 = new WriteableBitmapRenderer(cropFilter, cropdWbm))
                {
                    cropFilter.Filters = new IFilter[] { reframeFilter };
                    await renderer1.RenderAsync();

                    Final.Source = cropdWbm;
                }
            }

            bmp = null;
        }
    }
}