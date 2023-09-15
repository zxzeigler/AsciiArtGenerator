using System;
using System.IO;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AsciiLiberary
{

    public class AsciiArtGenerator
    {
        private Image<Rgba32> image;
        private readonly char[] asciiPixels = { ' ', '.', '-', '+', '*', 'w', 'G', 'H', 'M', '#', '&', '%' };
        private const double RedFactor = 0.241;
        private const double GreenFactor = 0.691;
        private const double BlueFactor = 0.068;

        public AsciiArtGenerator(){}

        public void SetNewImage(Stream imageStream)
        {
            this.image = Image.Load<Rgba32>(imageStream);
        }

        public string MakeAsciiArt(double imgScaleDivision, int degreesRotated)
        {
            var rotatedImg = RotateImageBy90Degrees(image, degreesRotated);
            var img = ResizeImage(rotatedImg, (int)(image.Width / imgScaleDivision), (int)(image.Height / imgScaleDivision));
            var sb = new StringBuilder();

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var color = img[x, y];
                    var brightness = GetBrightness(color);
                    var idx = brightness / 255 * (asciiPixels.Length - 1);
                    var pxl = asciiPixels[(int)Math.Round(idx)];
                    sb.Append(pxl);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private double GetBrightness(Rgba32 c)
        {
            return Math.Sqrt(
                (c.R * c.R * RedFactor) +
                (c.G * c.G * GreenFactor) +
                (c.B * c.B * BlueFactor)
            );
        }

        private Image<Rgba32> ResizeImage(Image<Rgba32> imgToResize, int width, int height)
        {
            // Adjust the height for ASCII character aspect ratio
            height = (int)(height * 0.5);  // Adjust this factor as needed

            imgToResize.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Stretch
            }));

            return imgToResize;
        }

        private Image<Rgba32> RotateImageBy90Degrees(Image<Rgba32> imgToRotate, int degrees)
        {
            imgToRotate.Mutate(x => x.Rotate(degrees));
            return imgToRotate;
        }
    }
}
