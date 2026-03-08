using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Authorization
{
    public class PolicyNames
    {
        public const string RequireAdminRole = "RequireAdminRole";
        public const string RequireUserRole = "RequireUserRole";
        public const string RequireManagerRole = "RequireManagerRole";

        public const string CanViewOwnOrders = "CanViewOwnOrders";
        public const string CanViewAllOrders = "CanViewAllOrders";
        public const string CanCreateOrder = "CanCreateOrder";
        public const string CanUpdateOrderStatus = "CanUpdateOrderStatus";
        public const string CanCancelOrder = "CanCancelOrder";
    }
}