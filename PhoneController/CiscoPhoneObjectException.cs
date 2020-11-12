using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCIERantsPhoneControl
{
    public class CiscoPhoneObjectException : Exception
    {
        public CiscoPhoneObject LastFaultObject;
        public string LastErrorMessage;

        public CiscoPhoneObjectException(string CustomErrorMessage, CiscoPhoneObject faultObject, Exception e)
        {
            LastFaultObject = faultObject;
            LastErrorMessage = CustomErrorMessage;
        }


    }
}
