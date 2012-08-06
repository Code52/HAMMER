using System;

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

        public string ConfigFile { get; set; }

        public Exception Exception { get; private set; }
    }
}