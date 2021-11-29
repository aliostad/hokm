using System;

namespace Hokm
{
    public class InvalidPlayException : Exception
    {
        public InvalidPlayException() : this("Invalid card played.")
        {
            
        }
        
        public InvalidPlayException(string error) : base(error)
        {
            
        }
    }
}