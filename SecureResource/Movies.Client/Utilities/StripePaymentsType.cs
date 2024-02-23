using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Utilities
{
    public static class StripePaymentsType
    {
        public static List<string> GetPaymentTypes()
        {
            const string types = "card, acss_debit, affirm, afterpay_clearpay, alipay, au_becs_debit, bacs_debit, bancontact, blik, boleto, cashapp, customer_balance, eps, fpx, giropay, grabpay, ideal, klarna, konbini, link, oxxo, p24, paynow, paypal, pix, promptpay, sepa_debit, sofort, us_bank_account, wechat_pay, revolut_pay";
            List<string> result = new List<string>();
            result = types.Split(',').ToList();
            return result;
        }
    }
}
