using iBank.Scanner.Data;

using System;

namespace iBank.Scanner
{
    public class MrzProcessedEventArgs : EventArgs
    {
        public Mrz Result { get; }

        public MrzProcessedEventArgs(Mrz result)
        {
            Result = result;
        }
    }
}