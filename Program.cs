using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageFadeOut
{
    public class Program
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option("-i|--input", Description = "The input image")]
        [FileExists]
        [Required]
        public string InputImage { get; }

        [Option("-s|--start", Description = "FadeOut start point (default: 0)")]
        public int Start { get; } = 0;

        [Option("-d|--direction", Description = "FadeOut direction (Top, Bottom, Left, Right) (default: Bottom)")]
        public FadeOutDirection Direction { get; } = FadeOutDirection.Bottom;

        [Argument(0, Description = "The output image")]
        public string OutputImage { get; } = "output.png";

        Bitmap sourceImage;
        Bitmap outputImage;

        private void OnExecute()
        {
            sourceImage = new Bitmap(Bitmap.FromFile(InputImage));

            switch (Direction)
            {
                case FadeOutDirection.Top:
                    sourceImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case FadeOutDirection.Left:
                    sourceImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case FadeOutDirection.Right:
                    sourceImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
            }

            outputImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            float opacity = 1;
            float opStep = opacity / (sourceImage.Height - Start);

            for (int i = 0; i < sourceImage.Height; i++)
            {
                Draw(opacity, i);
                if (i > Start) opacity -= opStep;
            }

            sourceImage.Dispose();

            switch (Direction)
            {
                case FadeOutDirection.Top:
                    outputImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case FadeOutDirection.Left:
                    outputImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case FadeOutDirection.Right:
                    outputImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            outputImage.Save(OutputImage, ImageFormat.Png);
            outputImage.Dispose();
        }

        private void Draw(float opacity, int i)
        {
            using (var g = Graphics.FromImage(outputImage))
            {
                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = opacity;

                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Default);

                g.DrawImage(sourceImage, new Rectangle(0, i, sourceImage.Width, 1), 0, i, sourceImage.Width, 1, GraphicsUnit.Pixel, ia);
            }
        }
    }

    public enum FadeOutDirection
    {
        Top, Left, Bottom, Right
    }
}
