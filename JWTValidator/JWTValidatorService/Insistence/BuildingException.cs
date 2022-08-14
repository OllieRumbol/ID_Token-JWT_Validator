using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTValidatorService
{
    public class BuildingException : Exception
    {
        public BuildingException()
        {
        }

        public BuildingException(String message)
            : base(message)
        {
        }

        public BuildingException(String message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
