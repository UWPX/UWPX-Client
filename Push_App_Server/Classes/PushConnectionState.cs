using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Push_App_Server.Classes
{
    public enum PushConnectionState
    {
        NOT_STARTED,
        SEND_DISCO_REQUEST,
        RECEIVED_DISCO_RESULT,
        DONE,
        ERROR
    }
}
