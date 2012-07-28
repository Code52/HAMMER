using System;
using System.ComponentModel.DataAnnotations;

namespace HAMMER.Pants
{
    public class PantsArgs
    {
        public PantsArgs()
        {
            
        }

        public PantsArgs(Exception exception)
        {
            Exception = exception;
        }

        [Required]
        public string Colour { get; set; }

        public string InputFile { get; set; }

        public string OutputFile { get; set; }

        public Exception Exception { get; private set; }
    }
}