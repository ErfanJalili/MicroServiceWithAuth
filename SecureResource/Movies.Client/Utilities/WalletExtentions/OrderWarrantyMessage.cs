using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Utilities.WalletExtentions
{
    public static class OrderWarrantyMessage
    {
        public static string CompleteOrderMessage(string orderCode)
        {
            return $"OrderCode : {orderCode} has been completed , Admins check your quest after that you can withdraw after 10  days . ";
        }
        public static string PendingMessage(string orderCode)
        {
            return $"OrderCode : {orderCode} has been Pending , Admins check your quest after that you can withdraw after 10  days . ";
        }
        public static string FrozenMessage(string orderCode)
        {
            return $"OrderCode : {orderCode} has been frozen , you can withdraw after 10  days . ";
        }

        public static string DenyByAdminMessage(string code, string reason)
        {
            return $"OrderCode : {code} has been denied by admin , Admin says : {reason} . ";
        }
    }
}
