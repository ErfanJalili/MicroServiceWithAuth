using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Utilities.WalletExtentions
{
    public static class WalletTransactionMessage
    {
        public static string CompletedOrderToPayMessage(string buyyer, string orderCode, string booster, string admin, decimal value)
        {
            return $"{buyyer} as buyyer with orderCode : {orderCode} has successfully finished by booster : {booster} , all these prosses handle by admin : {admin} , Final price to pay booster : {value} ";
        }

        public static string CongraulationBalanceCharged(string orderCode)
        {
            return $"Congratulation , order with :{orderCode} Completed Successfully , now you can withdraw! ";
        }
        public static string UpdateWalletByUserId(long changerId, long userId, decimal amount, string description)
        {
            return $"{changerId} change user wallet with userId : {userId} by amount : {amount} ,Reason : {description}";
        }
        public static string OrderPayed(string orderCode, decimal amount)
        {
            return $"Congratulation , order with code :{orderCode} and {amount} has been withdrawn from your account in : {DateTime.Now.ToString("mm/dd/yyyy")} ! ";
        }

        public static string CongraulationWithdrawRequestAcceptedAndPaid(long systemTransactionId, decimal amount)
        {
            return $"Your withdraw request with trasaction : {systemTransactionId} has been done with amount : {amount} ,you wallet updated successfully.";
        }
    }
}
