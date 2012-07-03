using System.ComponentModel.DataAnnotations;

namespace HAMMER.Pants
{
    public class PantsArgs
    {
        [Required]
        public string Colour { get; set; }

        public string InputFile { get; set; }

        public string OutputFile { get; set; }

        public bool Generate { get; set; }
    }
}