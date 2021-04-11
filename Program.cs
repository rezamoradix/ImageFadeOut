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

            outputImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            float opacity = 1;
            float opStep = opacity / ((Direction is FadeOutDirection.Bottom or FadeOutDirection.Top ? sourceImage.Height : sourceImage.Width) - Start);

            switch (Direction)
            {
                case FadeOutDirection.Top:
                    if (Start != 0)
                        using (var g = Graphics.FromImage(outputImage))
                            g.DrawImage(sourceImage, new Rectangle(0, sourceImage.Height - Start, sourceImage.Width, Start), 0, sourceImage.Height - Start, sourceImage.Width, Start, GraphicsUnit.Pixel);

                    for (int i = sourceImage.Height - Start; i > 0; i--)
                    {
                        Draw(opacity, i);
                        opacity -= opStep;
                    }
                    break;
                case FadeOutDirection.Left:
                    if (Start != 0)
                        using (var g = Graphics.FromImage(outputImage))
                            g.DrawImage(sourceImage, new Rectangle(sourceImage.Width - Start, 0, Start, sourceImage.Height), sourceImage.Width - Start, 0, Start, sourceImage.Height, GraphicsUnit.Pixel);

                    for (int i = sourceImage.Width - Start; i > 0; i--)
                    {
                        DrawH(opacity, i);
                        opacity -= opStep;
                    }
                    break;
                case FadeOutDirection.Right:
                    if (Start != 0)
                        using (var g = Graphics.FromImage(outputImage))
                            g.DrawImage(sourceImage, new Rectangle(0, 0, Start, sourceImage.Height), 0, 0, Start, sourceImage.Height, GraphicsUnit.Pixel);

                    for (int i = Start; i < sourceImage.Width; i++)
                    {
                        DrawH(opacity, i);
                        opacity -= opStep;
                    }
                    break;

                default:
                    if (Start != 0)
                        using (var g = Graphics.FromImage(outputImage))
                            g.DrawImage(sourceImage, new Rectangle(0, 0, sourceImage.Width, Start), 0, 0, sourceImage.Width, Start, GraphicsUnit.Pixel);

                    for (int i = Start; i < sourceImage.Height; i++)
                    {
                        Draw(opacity, i);
                        opacity -= opStep;
                    }
                    break;
            }

            outputImage.Save(OutputImage, ImageFormat.Png);
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
        private void DrawH(float opacity, int i)
        {
            using (var g = Graphics.FromImage(outputImage))
            {
                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = opacity;

                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Default);

                g.DrawImage(sourceImage, new Rectangle(i, 0, 1, sourceImage.Height), i, 0, 1, sourceImage.Height, GraphicsUnit.Pixel, ia);
            }
        }
    }

    public enum FadeOutDirection
    {
        Top, Left, Bottom, Right
    }
}
