using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Authorization
{
    public class Permissions
    {
        public const string ViewOwnOrders = "Orders.ViewOwn";
        public const string ViewAllOrders = "Orders.ViewAll";
        public const string CreateOrder = "Orders.Create";
        public const string UpdateOrderStatus = "Orders.UpdateStatus";
        public const string CancelOrder = "Orders.Cancel";
    }

     public static class RolePermissions
    {
        public static readonly Dictionary<string, List<string>> Mappings = new()
        {
            {
                "Admin", new List<string>
                {
                    Permissions.ViewOwnOrders,
                    Permissions.ViewAllOrders,
                    Permissions.CreateOrder,
                    Permissions.UpdateOrderStatus,
                    Permissions.CancelOrder,
                }
            },
            {
                "User", new List<string>
                {
                    Permissions.ViewOwnOrders,
                    Permissions.CreateOrder,
                    Permissions.CancelOrder,
                }
            },
            {
                "Manager", new List<string>
                {
                    Permissions.UpdateOrderStatus,
                }
            }
        };

        public static List<string> GetPermissionsForRole(string role)
        {
            return Mappings.TryGetValue(role, out var permissions) 
                ? permissions 
                : new List<string>();
        }
    }

}