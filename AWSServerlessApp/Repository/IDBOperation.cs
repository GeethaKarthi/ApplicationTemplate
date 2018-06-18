//using AWSServerlessApp.CustomModels;
using AWSServerlessApp.CustomModels;
using AWSServerlessApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSServerlessApp.Repository
{
    public interface IDBOperation
    {
        List<AspNetUsers> GetUsers();
       AspNetUserPinModel IsValidUserPin(long Pin);
    }
}
