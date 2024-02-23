using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Utilities.WalletExtentions
{
    public static class WalletCashBackMessages
    {
        public static string CashBackCharged(string orderCode, decimal cashBack)
        {
            return $" order with :{orderCode} has cash back with amount : {cashBack} , now you can see in cash backs!";
        }
        public static string RejectdCashBackAddedAgain(string orderCode, decimal cashBack)
        {
            return $" order with :{orderCode} has been removed from your orders , Cashback : {cashBack} , return to your wallet!";
        }
        public static string CompleteCashBackUsed(string orderCode, decimal cashBack)
        {
            return $" order with :{orderCode} has been completely payd by wallet with cashback amount : {cashBack} !";
        }
        public static string PayWithCashBack(string orderCode, decimal cashBack)
        {
            return $" order with :{orderCode} has been payd by wallet with cashback amount : {cashBack} !";
        }
        public static string WithDrawCashBack(string orderCode, decimal cashBack)
        {
            return $" order with code :{orderCode} has been used cashback, amount : {cashBack} !";
        }
    }
}
