using System;

namespace iBank.Scanner
{
    public class MrzRecognizedEventArgs : EventArgs
    {
        public string GivenNames { get; }
        public string SurNames { get; }
        public string MrzCode { get; }

        public MrzRecognizedEventArgs(string givenNames, string surNames, string mrzCode)
        {
            GivenNames = givenNames;
            SurNames = surNames;
            MrzCode = mrzCode;
        }
    }
}