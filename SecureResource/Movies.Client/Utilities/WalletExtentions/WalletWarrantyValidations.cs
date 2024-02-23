using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Utilities.WalletExtentions
{
    public static class WalletWarrantyValidations
    {
        public static int WarrantyDays(DateTime date)
        {
            var currentDate = DateTime.Now;
            var createdWarrantyDate = date;
            return (currentDate - createdWarrantyDate).Days;
        }
    }
}
