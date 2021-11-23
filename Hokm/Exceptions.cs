using System;

namespace Hokm
{
    public class InvalidPlayException : Exception
    {
        public InvalidPlayException() : base("Invalid Card played")
        {
            
        }
    }
}