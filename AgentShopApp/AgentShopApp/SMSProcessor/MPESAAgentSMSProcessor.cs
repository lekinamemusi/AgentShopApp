using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using AgentShopApp.Data;
using AgentShopApp.Data.Model;
using System.Linq;

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
            resultStoreData.TransactionType = await getTransactionType("Purchase Airtime", 1);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;
            //OAO0KWM4LQ confirmed. You bought Ksh50.00 of airtime for 254712106254 on 24/1/20 at 9:09 AM.New  balance is Ksh27, 147.00.Use current M - PESA PIN to activate M - PESA if you change sim
            //treat like deposit ask if money was give
            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.IndexOf("Ksh") + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.IndexOf(' ') + 1;//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the phone number for which this was bought
            startIndex = stringManipulate.IndexOf("for") + 4;//so that we begin from where the phone is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.IndexOf(' ') + 1;//find the first space 
            var phoneNumber = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.ClientData = await getClientDataFromPhone(phoneNumber);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.IndexOf("on") + 3;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.IndexOf('.');//find the first space 
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);

            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string


            return resultStoreData;
        }
        public async Task<SMSMessageStoreData> ProcessCustomerDeposit(SMSMessageStore rawMessage)
        {
            var resultStoreData = new SMSMessageStoreData();

            resultStoreData.SMSMessageStore = rawMessage;
            resultStoreData.TransactionType = await getTransactionType("Customer Deposit", 2);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;
            //OAO4KVPDQ4 Confirmed. On 24/1/20 at 8:26 AM Take Ksh1,100.00 cash from PASCALENE W MAINA Your M-PESA float balance is Ksh27,197.00.
            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.IndexOf(". On") + 5;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.IndexOf(" Take");//find the index of Take
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.IndexOf("Ksh") + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the client name
            startIndex = stringManipulate.IndexOf("from") + 5;//so that we begin from where the phone is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.IndexOf(" Your");//find the first space 
            var clientUniqueName = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.ClientData = await getDefaultCustomerForDeposit();
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            resultStoreData.Narration = clientUniqueName;//this in narration will help shift this record to its right place in ledgers
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string


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

        private async Task<ClientData> getClientDataFromNameAndPhone(string clientUniqueName, string clientPhoneNumber)
        {
            string msisdn = getMSISDNFromPhone(clientPhoneNumber);

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

        private async Task<TransactionType> getTransactionType(string transactionType, int value)
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

        public async Task<SMSMessageStoreData> ProcessCustomerWithdraw(SMSMessageStore rawMessage)
        {
            var resultStoreData = new SMSMessageStoreData();

            resultStoreData.SMSMessageStore = rawMessage;
            resultStoreData.TransactionType = await getTransactionType("Customer Withdraw", 3);
            resultStoreData.TransactionTypeId = resultStoreData.TransactionType.Id;
            //OAN3KNQZC7 Confirmed. on 23/1/20 at 7:51 PM Give Ksh1,530.00 to JAPHET MAINA MBOGO 0720435807. New M-PESA float balance is Ksh28,297.00
            //treat like deposit ask if money was give
            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            resultStoreData.TransactionId = transactionCode;
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);

            //get the transaction time number for which this was bought
            startIndex = stringManipulate.IndexOf(". on") + 5;//so that we begin from where the date time is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.IndexOf(" Give");//find the index of Take
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent).Trim(); //and copy that as amount to variable
            resultStoreData.TransactionTime = getFormatedDate(transactionTime);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.IndexOf("Ksh") + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.IndexOf(' ');//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent).Replace(",", "").Trim(); //and copy that as amount to variable
            resultStoreData.Amount = Convert.ToDouble(transAmount);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string

            //get the client name
            startIndex = stringManipulate.IndexOf("to") + 3;//so that we begin from where the phone is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.LastIndexOf(". ");//find the last index of space 
            stringManipulate = stringManipulate.Substring(startIndex, endIndexCurrent); //and copy that as amount to variable
            endIndexCurrent = stringManipulate.LastIndexOf(" ");
            var clientUniqueName = stringManipulate.Substring(0, endIndexCurrent).Trim();

            stringManipulate = stringManipulate.Substring(endIndexCurrent + 1);
            //get the client name
            var clientPhoneNumber = stringManipulate;

            resultStoreData.ClientData = await getClientDataFromNameAndPhone(clientUniqueName, clientPhoneNumber);
            resultStoreData.ClientDataId = resultStoreData.ClientData.Id;
            return resultStoreData;
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
            else if (rawMessage.TextMessage.ToLower().Contains("Give".ToLower()))
                return await this.ProcessCustomerWithdraw(rawMessage);
            else if (rawMessage.TextMessage.ToLower().Contains("Take".ToLower()))
                return await this.ProcessCustomerDeposit(rawMessage);
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
}
