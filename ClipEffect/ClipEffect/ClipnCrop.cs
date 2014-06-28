using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ClipEffect
{
    class ClipnCrop : CustomEffectBase
    {
        public Windows.Foundation.Size CropSize { get; set; }
        public Windows.Foundation.Point Corner { get; set; }

        public IImageProvider Mask { get; set; }

        int cropMinWidth = int.MaxValue, cropMinHeight = int.MaxValue, cropMaxWidth = int.MinValue, cropMaxHeight = int.MinValue;

        public ClipnCrop(IImageProvider source, IImageProvider mask) :
            base(source, false)
        {
            Mask = mask;
        }

        protected override void OnProcess(PixelRegion sourcePixelRegion, PixelRegion targetPixelRegion)
        {
            if (Mask == null)
            {
                //copy all pixel
                Array.Copy(sourcePixelRegion.ImagePixels, targetPixelRegion.ImagePixels, sourcePixelRegion.ImagePixels.Length);
                return;
            }

            //create buffer
            uint[] buffer = new uint[(int)(sourcePixelRegion.Bounds.Width * sourcePixelRegion.Bounds.Height)];
            
            var bitmapMask = new Bitmap(
                new Windows.Foundation.Size(sourcePixelRegion.Bounds.Width, sourcePixelRegion.Bounds.Height),
                ColorMode.Bgra8888,
                4*(uint)sourcePixelRegion.Bounds.Width,
                buffer.AsBuffer());

            //load buffer
            Mask.GetBitmapAsync(bitmapMask, OutputOption.Stretch).AsTask().Wait();

            //Process
            sourcePixelRegion.ForEachRow((index, width, pos) =>
            {
                for (int x = 0; x < width; ++x, ++index)
                {
                    if (buffer[index] > 128)
                    {
                        targetPixelRegion.ImagePixels[index] = sourcePixelRegion.ImagePixels[index];
                    }
                    else
                    {
                        targetPixelRegion.ImagePixels[index] = 0;
                    }
                }
            });

            for (int i = 0; i < sourcePixelRegion.ImageSize.Height; ++i)
                for (int j = 0; j < sourcePixelRegion.ImageSize.Width; ++j)
                {
                    int index = i * (int)sourcePixelRegion.ImageSize.Width + j;
                    if (buffer[index] > 128)
                    {
                        if (cropMinWidth > j) cropMinWidth = j;
                        if (cropMaxWidth < j) cropMaxWidth = j;
                        if (cropMinHeight > i) cropMinHeight = i;
                        if (cropMaxHeight < i) cropMaxHeight = i;
                    }
                }

            CropSize = new Windows.Foundation.Size(cropMaxWidth - cropMinWidth, cropMaxHeight - cropMinHeight);
            Corner = new Windows.Foundation.Point(cropMinWidth, cropMinHeight);
        }

    }
}
