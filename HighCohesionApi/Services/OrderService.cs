using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HighCohesionApi.Services
{
    //High Cohesion Olmayan
    public class OrderService
    {
        public async Task<bool> CreateOrder(object order)
        {
            return true;
        }        

        public async Task<bool> UpdateUser(object user)
        {
            return true;
        }
    }
}