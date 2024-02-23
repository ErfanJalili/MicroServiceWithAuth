using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Utilities.WalletExtentions
{
    public static class WalletBalanceMessages
    {
        public static string CongraulationBalanceCharged(string orderCode)
        {
            return $"Congratulation , order with :{orderCode} Completed Successfully , now you can withdraw! ";
        }
    }
}
