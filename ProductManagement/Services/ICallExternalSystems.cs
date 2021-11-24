using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Services
{
  public  interface ICallExternalSystemService
    {
        Task<bool> SubmitOrderToExternalSystem(string jsonOrder);

    }
}
