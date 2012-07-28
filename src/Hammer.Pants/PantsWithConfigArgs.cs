using System;
using System.ComponentModel.DataAnnotations;

namespace HAMMER.Pants
{
    public class PantsWithConfigArgs
    {
        public PantsWithConfigArgs()
        {

        }

        public PantsWithConfigArgs(Exception exception)
        {
            Exception = exception;
        }

        [Required]
        public string ConfigFile { get; set; }

        public Exception Exception { get; private set; }
    }
}