using System;

namespace Sweep.Core.Marking.Representation
{
    public partial class TrackNameModel
    {
        public class InvalidNameFormatException : Exception
        {
            public InvalidNameFormatException(string message) : base(message)
            {

            }
        }
    }
}
