using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using AgentShopApp.Data;
using AgentShopApp.Data.Model;
using System.Linq;
using AgentShopApp.Configuration;

namespace AgentShopApp.SMSProcessor
{
    public class MPESAAgentSMSProcessor : ISMSProcessor
    {
        private readonly string[] dateFormats = new string[]
        {
            "d/M/yy \"at\" h:mm tt",
            "d/MM/yy \"at\" h:mm tt",

            "dd/M/yy \"at\" h:mm tt",
            "dd/MM/yy \"at\" h:mm tt"
        };

        private DateTime getFormatedDate(string dateValue)
        {
            DateTime outResult;
            DateTime.TryParseExact(dateValue, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out outResult);
            return outResult;
        }

        public async Task<SMSMessageStoreData> ProcessPurchaseAirtime(SMSMessageStore rawMessage)
        {
            var resultStoreData = new SMSMessageStoreData();

            resultStoreData.SMSMessageStore = rawMessage;
            resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.PurchaseAirtime);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;
            //OAO0KWM4LQ confirmed. You bought Ksh50.00 of airtime for 254712106254 on 24/1/20 at 9:09 AM.New  balance is Ksh27, 147.00.Use current M - PESA PIN to activate M - PESA if you change sim
            //treat like deposit ask if money was give
            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.ToLower().IndexOf("Ksh".ToLower()) + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ') + 1;//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the phone number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf("for".ToLower()) + 4;//so that we begin from where the phone is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ') + 1;//find the first space 
            var phoneNumber = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.ClientData = await getClientDataFromPhone(phoneNumber);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf("on".ToLower()) + 3;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf('.');//find the first space 
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);

            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string
            return resultStoreData;
        }
        public async Task<SMSMessageStoreData> ProcessCustomerDeposit(SMSMessageStore rawMessage)
        {
            var resultStoreData = new SMSMessageStoreData();

            resultStoreData.SMSMessageStore = rawMessage;
            resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.CustomerDeposit);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;
            //OAO4KVPDQ4 Confirmed. On 24/1/20 at 8:26 AM Take Ksh1,100.00 cash from PASCALENE W MAINA Your M-PESA float balance is Ksh27,197.00.
            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf(". On".ToLower()) + 5;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(" Take".ToLower());//find the index of Take
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.ToLower().IndexOf("Ksh".ToLower()) + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the client name
            startIndex = stringManipulate.ToLower().IndexOf("from".ToLower()) + 5;//so that we begin from where the phone is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(" Your".ToLower());//find the first space 
            var clientUniqueName = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.ClientData = await getDefaultCustomerForDeposit();
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            resultStoreData.Narration = clientUniqueName;//this in narration will help shift this record to its right place in ledgers
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string


            return resultStoreData;
        }
        public async Task<SMSMessageStoreData> ProcessCustomerWithdraw(SMSMessageStore rawMessage)
        {
            var resultStoreData = new SMSMessageStoreData();

            resultStoreData.SMSMessageStore = rawMessage;

            //withdraw saf no
            //OAN3KNQZC7 Confirmed. on 23/1/20 at 7:51 PM Give Ksh1,530.00 to JAPHET MAINA MBOGO 0720435807. New M-PESA float balance is Ksh28,297.00
            //withdraw non saf
            //OA314OGB3N  Confirmed.On 3/1/20 at 11:18 AM.Give Ksh5,100.00 cash to +254734373935.New M-PESA float balance is Ksh23,103.00
            //treat like deposit ask if money was give
            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf("Confirmed.On".ToLower());//so that we begin from where the date time is exactly
            bool unRegisteredWithDraw = false;
            if (startIndex < 0)
            {
                startIndex = stringManipulate.ToLower().IndexOf("Confirmed. on".ToLower()) + 14;
            }
            else
            {
                unRegisteredWithDraw = true;
                startIndex += 13;//for non customer sakes
            }
            if (unRegisteredWithDraw == true)
                resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.CustomerWithdrawUnRegistered);
            else
                resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.CustomerWithdraw);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;

            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf("Give".ToLower());//find the index of Take
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent).Trim().Replace(".", ""); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.ToLower().IndexOf("Ksh".ToLower()) + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the client name
            startIndex = stringManipulate.ToLower().IndexOf("to".ToLower()) + 3;//so that we begin from where the phone is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf("New".ToLower());//find index of New
            stringManipulate = stringManipulate.Substring(startIndex, endIndexCurrent); //and copy that as amount to variable
            endIndexCurrent = stringManipulate.ToLower().LastIndexOf(" ");
            if (endIndexCurrent < 0)
                endIndexCurrent = stringManipulate.Length;
            var clientUniqueName = stringManipulate.Substring(0, endIndexCurrent).Trim().Replace(".", "");

            //incase the unique name starts with a + then this a non saf customer withdraw
            var clientPhoneNumber = string.Empty;
            if (clientUniqueName.StartsWith("+"))
            {
                //non saf withdrawal
                clientUniqueName = clientUniqueName.Replace("+", "");
                clientPhoneNumber = clientUniqueName;
            }
            else
            {
                //for a registered saf number withdrawal
                startIndex = 0;
                endIndexCurrent = stringManipulate.LastIndexOf(" ");
                stringManipulate = stringManipulate.Substring(startIndex, endIndexCurrent);///remove upto  New

                endIndexCurrent = stringManipulate.LastIndexOf(" ");
                clientUniqueName = stringManipulate.Substring(startIndex, endIndexCurrent);// Separates the name from phone

                stringManipulate = stringManipulate.Replace(clientUniqueName, "");//delete the copied amount from this string
                //get the client name
                clientPhoneNumber = stringManipulate.Trim().Replace(".", "");
            }

            resultStoreData.ClientData = await getClientDataFromNameAndPhone(clientUniqueName, clientPhoneNumber);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            return resultStoreData;
        }

        //nt
        private async Task<SMSMessageStoreData> ProcessTransferedFromWorkingToFloat(SMSMessageStore rawMessage)
        {
            //OA354XX90L Confirmed. Ksh500.00   transferred from Working to Float.New Working Balance is Ksh468.03.New Float Balance is Ksh1,084.00 on 3/1/20 at 4:58 PM.
            var resultStoreData = new SMSMessageStoreData();

            resultStoreData.SMSMessageStore = rawMessage;
            resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.WorkingToFloat);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;

            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //strip string to KES point
            startIndex = stringManipulate.ToLower().IndexOf("Ksh".ToLower()) + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf("on ".ToLower()) + 4;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            var transactionTime = stringManipulate.Substring(startIndex).Trim().Replace(".", ""); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);

            resultStoreData.ClientData = await getClientDataFromNameAndPhone(genericCustomerName, genericCustomerPhone);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            return resultStoreData;
        }
        private async Task<SMSMessageStoreData> ProcessTransferedFromFloatToWorking(SMSMessageStore rawMessage)
        {
            //OA2545YOU3 Confirmed. Ksh16,000.00  transferred from Float Ac.to Working Ac..New Working Ac. Balance is Ksh32,968.03.New Float Balance is Ksh50,003.00 on 2/1/20 at 4:26 PM
            var resultStoreData = new SMSMessageStoreData();

            resultStoreData.SMSMessageStore = rawMessage;
            resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.FloatToWorking);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;

            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //strip string to KES point
            startIndex = stringManipulate.ToLower().IndexOf("Ksh".ToLower()) + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf("on ".ToLower()) + 4;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            var transactionTime = stringManipulate.Substring(startIndex).Trim().Replace(".", ""); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);

            resultStoreData.ClientData = await getClientDataFromNameAndPhone(genericCustomerName, genericCustomerPhone);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            return resultStoreData;

        }
        private async Task<SMSMessageStoreData> ProcessBuyFloatFromAgent(SMSMessageStore rawMessage)
        {
            //NLO1WBXPWD Business Deposit Confirmed on 24/12/19 at 3:59 PM. Give Ksh130,000.00 cash to 147101 - Ropem Telcom Bethsaida Ziwani Kariokor. New Working balance is Ksh147,550.69

            var resultStoreData = new SMSMessageStoreData();

            resultStoreData.SMSMessageStore = rawMessage;
            resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.BuyFloatFromAgent);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;
            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf(" on".ToLower()) + 4;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(". Give".ToLower());//find the index of Take
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.ToLower().IndexOf("Ksh".ToLower()) + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the agent name and phone phone number
            startIndex = stringManipulate.ToLower().IndexOf("to") + 3;//so that we begin from where the phone is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from number begin point to the end
            endIndexCurrent = stringManipulate.IndexOf(" - ");
            var clientPhoneNumber = stringManipulate.Substring(startIndex, endIndexCurrent); //and copy that as amount to variable
            stringManipulate = stringManipulate.Substring(endIndexCurrent + 3);//trim to remove the number
            endIndexCurrent = stringManipulate.IndexOf(".");
            var clientUniqueName = stringManipulate.Substring(0, endIndexCurrent).Trim();//begin from pos 3 to skip ( - )

            resultStoreData.ClientData = await getClientDataFromNameAndPhone(clientUniqueName, clientPhoneNumber, true);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            return resultStoreData;
        }
        private async Task<SMSMessageStoreData> ProcessSaleFloatToAgent(SMSMessageStore rawMessage)
        {
            //NLV92KJXU5 Business Deposit Confirmed on 31/12/19 at 3:43 PM. Take Ksh20,100.00 cash from 105104 - Jays Call Mobile Accra Trading Centre Ground Floor Nairobi. Your Float balance is Ksh51,617.00

            var resultStoreData = new SMSMessageStoreData();

            resultStoreData.SMSMessageStore = rawMessage;
            resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.SaleFloatToAgent);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;

            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf(" on".ToLower()) + 4;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(". Take".ToLower());//find the index of Take
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.ToLower().IndexOf("Ksh".ToLower()) + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the agent name and phone phone number
            startIndex = stringManipulate.ToLower().IndexOf("from") + 5;//so that we begin from where the phone is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from number begin point to the end
            endIndexCurrent = stringManipulate.IndexOf(" - ");
            var clientPhoneNumber = stringManipulate.Substring(startIndex, endIndexCurrent); //and copy that as amount to variable

            stringManipulate = stringManipulate.Substring(endIndexCurrent + 3);//trim to remove the number
            endIndexCurrent = stringManipulate.IndexOf(".");
            var clientUniqueName = stringManipulate.Substring(0, endIndexCurrent).Trim();//begin from pos 3 to skip ( - )

            resultStoreData.ClientData = await getClientDataFromNameAndPhone(clientUniqueName, clientPhoneNumber, true);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            return resultStoreData;
        }
        private async Task<SMSMessageStoreData> ProcessCommissionPayment(SMSMessageStore rawMessage)
        {
            //OA1831LCUQ confirmed. Commission of Ksh17,807.19 paid to your Working account. Working Balance is Ksh16,968.03 on 1/1/20 at 2:10 AM
            var resultStoreData = new SMSMessageStoreData();
            resultStoreData.SMSMessageStore = rawMessage;
            resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.CommissionPayment);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;

            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);


            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.ToLower().IndexOf("Ksh".ToLower()) + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf("on ".ToLower()) + 4;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            var transactionTime = stringManipulate.Substring(startIndex).Trim().Replace(".", ""); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);

            resultStoreData.ClientData = await getClientDataFromNameAndPhone(genericCustomerName, genericCustomerPhone);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            return resultStoreData;
        }
        private async Task<SMSMessageStoreData> ProcessReverseTransaction(SMSMessageStore rawMessage)
        {
            //OA516AJI99 confirmed. Reversal of transaction OA566A7X9G has been successfully reversed on 5/1/20 at 1:14 PM and Ksh500.00 is debited from your M-PESA account. Your new float account balance is Ksh39,516.00
            var resultStoreData = new SMSMessageStoreData();
            resultStoreData.SMSMessageStore = rawMessage;
            if (rawMessage.TextMessage.ToLower().Contains("credited"))
                resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.ReverseTransactionCredited);
            else
                resultStoreData.TransactionType = await getTransactionType(GeneralConfiguration.TransactionTypeConfig.ReverseTransactionDebited);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;

            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);
            //get the reversed transaction and put it in narration
            //start from end of transaction
            startIndex = stringManipulate.ToLower().IndexOf("transaction".ToLower()) + 12;
            stringManipulate = stringManipulate.Substring(startIndex);//trim to start of next
            startIndex = 0;
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');//move to the begining of the first step
            var reversedEntry = stringManipulate.Substring(startIndex, endIndexCurrent);
            resultStoreData.Narration = reversedEntry;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.ToLower().IndexOf(" on".ToLower()) + 4;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//
            endIndexCurrent = stringManipulate.ToLower().IndexOf(" and".ToLower());//find the index of Take
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string


            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.ToLower().IndexOf("Ksh".ToLower()) + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.ToLower().IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            resultStoreData.ClientData = await getClientDataFromNameAndPhone(genericCustomerName, genericCustomerPhone);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;

            return resultStoreData;
        }

        private string getMSISDNFromPhone(string phoneNumber)
        {
            phoneNumber = phoneNumber.Trim();
            if (phoneNumber.StartsWith("254"))
                return phoneNumber;
            else if (phoneNumber.StartsWith("0"))
                return string.Format("254{0}", phoneNumber.Substring(1));
            throw new NotSupportedException("Sorry the phone number suplied could not be converted to an MSISDN");
        }
        private async Task<ClientData> getClientDataFromNameAndPhone(string clientUniqueName, string clientPhoneNumber, bool ensureMSISDNValidated = true)
        {
            string msisdn = clientPhoneNumber;
            if (ensureMSISDNValidated == false)
                msisdn = getMSISDNFromPhone(clientPhoneNumber);

            //check if the record is of a phone number registered without name and update it
            var result = await App.Database.DatabaseConnection.Table<ClientData>()
                .FirstOrDefaultAsync(r => r.ClientPhone == msisdn);
            if (result != null)
            {
                //just do an update incase of an msisdn only, incase it existed post it to the custome that has this number registered
                if (result.ClientName != clientUniqueName
                    && clientUniqueName != msisdn)
                    result.ClientName = clientUniqueName;//incase a true name was supplied the use it
                await App.Database.DatabaseConnection.UpdateAsync(result);
                return result;
            }
            else
            {
                var insertedClient = new ClientData
                {
                    ClientName = clientUniqueName,
                    ClientPhone = msisdn,
                    UnixTimeStamp = App.Database.GetUnixTimeStamp()
                };
                await App.Database.DatabaseConnection.InsertAsync(insertedClient);

                return insertedClient;
            }
        }
        private const string genericCustomerName = "Default Customer";
        private const string genericCustomerPhone = "254727000000";
        private async Task<ClientData> getDefaultCustomerForDeposit()
        {
            //this is a generic customer issue
            //use the generic customer details but 
            return await getClientDataFromNameAndPhone(MPESAAgentSMSProcessor.genericCustomerName, MPESAAgentSMSProcessor.genericCustomerPhone);

        }
        private async Task<ClientData> getClientDataFromPhone(string clientPhoneNumber)
        {
            var msisdn = getMSISDNFromPhone(clientPhoneNumber);

            return await getClientDataFromNameAndPhone(msisdn, msisdn);

        }
        private async Task<TransactionType> getTransactionType(string transactionType)
        {
            //check if the transaction type exist and return it if not then create it and return it
            var result = await App.Database.DatabaseConnection.Table<TransactionType>()
                .FirstOrDefaultAsync(r => r.Name == transactionType);

            if (result != null)
                return result;

            var insertedObject = new TransactionType
            {
                Name = transactionType,
                UnixTimeStamp = App.Database.GetUnixTimeStamp()
            };
            await App.Database.DatabaseConnection.InsertAsync(insertedObject);

            return insertedObject;
        }


        public async Task<SMSMessageStoreData> ProcessAsync(SMSMessageStore rawMessage)
        {
            #region MPESA AGENT message
            //BUY ARTIME MESSAGE
            //OAO0KWM4LQ confirmed. You bought Ksh50.00 of airtime for 254712106254 on 24 / 1 / 20 at 9:09 AM.New  balance is Ksh27, 147.00.Use current M - PESA PIN to activate M - PESA if you change sim
            //WITHDRAW MESSAGE
            //OAN3KNQZC7 Confirmed. on 23/1/20 at 7:51 PM Give Ksh1,530.00 to JAPHET MAINA MBOGO 0720435807. New M-PESA float balance is Ksh28,297.00
            //CUSTOMER DEPOSIT MESSAGE
            //OAO4KVPDQ4 Confirmed. On 24/1/20 at 8:26 AM Take Ksh1,100.00 cash from PASCALENE W MAINA Your M-PESA float balance is Ksh27,197.00.
            #endregion
            if (rawMessage.TextMessage.ToLower().Contains("airtime".ToLower()))
                return await this.ProcessPurchaseAirtime(rawMessage);
            else if (rawMessage.TextMessage.ToLower().Contains("Business Deposit".ToLower())
                && rawMessage.TextMessage.ToLower().Contains("Take".ToLower()))//must be up here as it has give and take
                return await this.ProcessSaleFloatToAgent(rawMessage);
            else if (rawMessage.TextMessage.ToLower().Contains("Business Deposit".ToLower())
               && rawMessage.TextMessage.ToLower().Contains("Give".ToLower()))//must be up here as it has give and take
                return await this.ProcessBuyFloatFromAgent(rawMessage);
            else if (rawMessage.TextMessage.ToLower().Contains("Give".ToLower()))
                return await this.ProcessCustomerWithdraw(rawMessage);//test this handles both registered and un registered number withdrawal
            else if (rawMessage.TextMessage.ToLower().Contains("Take".ToLower()))
                return await this.ProcessCustomerDeposit(rawMessage);
            else if (rawMessage.TextMessage.ToLower().Contains("Reversal".ToLower()))
                return await this.ProcessReverseTransaction(rawMessage);
            else if (rawMessage.TextMessage.ToLower().Contains("Commission".ToLower()))
                return await this.ProcessCommissionPayment(rawMessage);

            else if (rawMessage.TextMessage.ToLower().Contains("transferred from Float".ToLower()))
                return await this.ProcessTransferedFromFloatToWorking(rawMessage);
            else if (rawMessage.TextMessage.ToLower().Contains("transferred from Working".ToLower()))
                return await this.ProcessTransferedFromWorkingToFloat(rawMessage);

            throw new NotImplementedException("Sorry supplied message is not supported");
        }



        public async Task<SMSMessageStoreData> ProcessAndSaveAsync(SMSMessageStore rawMessage)
        {
            var proccessedSMS = await this.ProcessAsync(rawMessage);
            var saveToStore = await App.Database.DatabaseConnection.Table<SMSMessageStoreData>()
                .FirstOrDefaultAsync(r => r.TransactionId == proccessedSMS.TransactionId);
            if (saveToStore != null)
                return saveToStore;
            var smsMsgStoreIns = new SMSMessageStoreData
            {
                UnixTimeStamp = App.Database.GetUnixTimeStamp(),
                SMSMessageStoreId = rawMessage.Id,
                TransactionId = proccessedSMS.TransactionId,
                TransactionTime = proccessedSMS.TransactionTime,
                Amount = proccessedSMS.Amount,
                ClientDataId = proccessedSMS.ClientDataId,
                TransactionSynced = false,
                TransactionTypeId = proccessedSMS.TransactionTypeId,
            };
            await App.Database.DatabaseConnection.InsertAsync(smsMsgStoreIns);

            return smsMsgStoreIns;

        }

        public bool Support(string senderId)
        {
            if (string.IsNullOrEmpty(senderId))
                return false;
            return senderId.ToLower() == "MPESA".ToLower();
        }
    }


    //Reversal
    //OA516AJI99 confirmed.Reversal of transaction OA566A7X9G has been successfully reversed on 5/1/20 at 1:14 PM and Ksh500.00 is debited from your M-PESA account.Your new float account balance is Ksh39,516.00
    //Commission
    //OA1831LCUQ confirmed.Commission of Ksh17,807.19 paid to your Working account.Working Balance is Ksh16,968.03 on 1/1/20 at 2:10 AM
    //Selling Float To Agent
    //NLV92KJXU5 Business Deposit Confirmed on 31/12/19 at 3:43 PM.Take Ksh20,100.00 cash from 105104 - Jays Call Mobile Accra Trading Centre Ground Floor Nairobi.Your Float balance is Ksh51,617.00
    //Withdrawal from non saf non
    //OA314OGB3N  Confirmed.On 3/1/20 at 11:18 AM.Give Ksh5,100.00 cash to +254734373935.New M-PESA float balance is Ksh23,103.00
    //Buy Float
    //NLO1WBXPWD Business Deposit Confirmed on 24/12/19 at 3:59 PM.Give Ksh130,000.00 cash to 147101 - Ropem Telcom Bethsaida Ziwani Kariokor.New Working balance is Ksh147,550.69
    //From Float to working
    //OA2545YOU3 Confirmed.Ksh16,000.00  transferred from Float Ac.to Working Ac..New Working Ac.Balance is Ksh32,968.03.New Float Balance is Ksh50,003.00 on 2/1/20 at 4:26 PM
    //From working to float
    //OA354XX90L Confirmed.Ksh500.00   transferred from Working to Float.New Working Balance is Ksh468.03.New Float Balance is Ksh1,084.00 on 3/1/20 at 4:58 PM.

}
