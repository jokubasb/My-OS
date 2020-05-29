using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OS.Machine
{
    class ParsingException : Exception
    {
        public ParsingException()
        { }

        public ParsingException(string fileName, string message, int lineNo)
            : base(fileName+":"+lineNo+": "+"error: "+message)
        { }

        public ParsingException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
