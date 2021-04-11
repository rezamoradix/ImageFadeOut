using System;
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

        [Option("-o|--output", Description = "The output image")]
        public string OutputImage { get; } = "output.png";

        private void OnExecute()
        {
            Image source = Bitmap.FromFile(InputImage);
            Bitmap image = new Bitmap(source);
        }
    }
}
