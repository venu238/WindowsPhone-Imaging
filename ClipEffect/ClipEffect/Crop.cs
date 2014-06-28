using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipEffect
{
    class Crop : CustomEffectBase
    {
        
        public Windows.Foundation.Size CropSize { get; set; }
        public Windows.Foundation.Point Corner { get; set; }

        int cropMinWidth = int.MaxValue, cropMinHeight = int.MaxValue, cropMaxWidth = int.MinValue, cropMaxHeight = int.MinValue;


        public Crop(IImageProvider source) :
            base(source, false)
        {
        }

        protected override void OnProcess(PixelRegion sourcePixelRegion, PixelRegion targetPixelRegion)
        {
            for (int i = 0; i < sourcePixelRegion.ImageSize.Height; ++i)
                for (int j = 0; j < sourcePixelRegion.ImageSize.Width; ++j)
                {
                    int index = i * (int)sourcePixelRegion.ImageSize.Width + j;
                    if (sourcePixelRegion.ImagePixels[index] > 128)
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
