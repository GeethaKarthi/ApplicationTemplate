using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApp.LogProvider
{
    //
    // Summary:
    //     Defines logging severity levels.
    public enum LoggingEvents
    {
        AUTHENTICATE_USER = 1000,
        REGISTER_USER = 1001,
        VALIDATE_ITEM = 1002,
        LIST_ITEMS = 1005,
        GET_ITEM = 1010,
        INSERT_ITEM = 1015,
        UPDATE_ITEM = 1020,
        DELETE_ITEM = 1025
    }

}
