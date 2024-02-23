using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Utilities.WalletExtentions
{
    public static class WalletFundInHoldMessages
    {
        public static string FoundInHoldCharged(string orderCode)
        {
            return $" order with :{orderCode} Completed Successfully , now you can see in FoundInHold !";
        }
    }
}
