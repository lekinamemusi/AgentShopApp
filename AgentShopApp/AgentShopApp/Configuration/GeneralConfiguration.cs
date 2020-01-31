using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Configuration
{
    public static class GeneralConfiguration
    {
        public static class TransactionTypeConfig
        {

            public static string PurchaseAirtime = "Purchase Airtime";
            public static string CustomerDeposit = "Customer Deposit";
            public static string CustomerWithdraw = "Customer Withdraw";
            public static string WorkingToFloat = "Working To Float";
            public static string FloatToWorking = "Float To Working";
            public static string BuyFloatFromAgent = "Buy Float From Agent";
            public static string SaleFloatToAgent = "Sale Float To Agent";
            public static string CommissionPayment = "Commission Payment";
            public static string ReverseTransactionCredited = "Reverse Transaction Credited";
            public static string ReverseTransactionDebited = "Reverse Transaction Debited";

            public static string CustomerWithdrawUnRegistered = "Customer Withdraw Un Registered";
        }
    }
}
