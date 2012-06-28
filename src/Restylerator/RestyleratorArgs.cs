using System.ComponentModel.DataAnnotations;

namespace Restylerator
{
    public class RestyleratorArgs
    {
        [Required]
        public string Colour { get; set; }

        public string InputFile { get; set; }

        public string OutputFile { get; set; }

        public bool Generate { get; set; }
    }
}