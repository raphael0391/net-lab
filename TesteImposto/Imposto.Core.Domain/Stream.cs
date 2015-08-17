using System;
using System.IO;

namespace Imposto.Core.Domain
{
    internal class Stream
    {
        public static implicit operator Stream(FileStream v)
        {
            throw new NotImplementedException();
        }
    }
}