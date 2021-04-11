using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace ImageFadeOut
{
    public class Program
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option("-i|--input", Description = "The input image")]
        [FileExists]
        [Required]
        public string InputImage { get; }

        [Option("-s|--start", Description = "Fade out start point")]
        public int Start { get; } = 0;

        [Argument(0, Description = "The output image")]
        public string OutputImage { get; } = "output.png";

        private void OnExecute()
        {
            Image source = Bitmap.FromFile(InputImage);
            Bitmap image = new Bitmap(source);

            var b = new Bitmap(image.Width, image.Height);

            float opacity = 1;
            float opStep = opacity / (image.Height - Start);

            if (Start != 0)
                using (var g = Graphics.FromImage(b))
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, Start), 0, 0, image.Width, Start, GraphicsUnit.Pixel);

            for (int i = Start; i < image.Height; i++)
            {
                using (var g = Graphics.FromImage(b))
                {
                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix33 = opacity;

                    ImageAttributes ia = new ImageAttributes();
                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Default);

                    g.DrawImage(image, new Rectangle(0, i, image.Width, 1), 0, i, image.Width, 1, GraphicsUnit.Pixel, ia);
                }
                opacity -= opStep;
            }

            b.Save(OutputImage, ImageFormat.Png);
        }
    }
}
