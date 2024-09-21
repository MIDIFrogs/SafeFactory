using Compunet.YoloV8;
using Compunet.YoloV8.Data;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeFactory.VideoCapture
{
    internal class FrameProcessor
    {
        private YoloPredictor predictor;
        private YoloConfiguration configuration;

        public FrameProcessor()
        {
        }

        public async Task<YoloResult<Segmentation>> SegmentAsync(Image image)
        {
            return await predictor.SegmentAsync(image, configuration);
        }

        public float CompareSegments(Segmentation previous, Segmentation next)
        {
            throw new NotImplementedException();
        }
    }
}
