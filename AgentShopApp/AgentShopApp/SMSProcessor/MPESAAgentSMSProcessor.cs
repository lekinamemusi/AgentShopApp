using System;
using System.Collections.Generic;
using System.Text;
using AgentShopApp.Data;

namespace AgentShopApp.SMSProcessor
{
    public class MPESAAgentSMSProcessor : ISMSProcessor
    {
        public SMSMessageStoreData ProcessPurchaseAirtime(SMSMessageStore rawMessage)
        {
            var resultStoreData = new SMSMessageStoreData();
            //OAO0KWM4LQ confirmed. You bought Ksh50.00 of airtime for 254712106254 on 24/1/20 at 9:09 AM.New  balance is Ksh27, 147.00.Use current M - PESA PIN to activate M - PESA if you change sim
            //treat like deposit ask if money was give
            var stringManipulate = rawMessage.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.IndexOf(' ') + 1;
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent);
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);
            
            //get the amount
            //strip string to KES point
            startIndex = stringManipulate.IndexOf("Ksh") + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from amount to the end
            endIndexCurrent = stringManipulate.IndexOf(' ') + 1;//find the first space 
            var transAmount = stringManipulate.Substring(startIndex, endIndexCurrent); //and copy that as amount to variable
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string
            
            //get the phone number for which this was bought
            startIndex = stringManipulate.IndexOf("for") + 3;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.IndexOf(' ') + 1;//find the first space 
            var phoneNumber = stringManipulate.Substring(startIndex, endIndexCurrent); //and copy that as amount to variable
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string
            
            //get the transaction time number for which this was bought
            startIndex = stringManipulate.IndexOf("on") + 2;//so that we begin from where the amount is exactly
            stringManipulate = stringManipulate.Substring(startIndex);//begin from the end of ksh at amount begin point
            startIndex = 0;//now the string starts from phone number begin point to the end
            endIndexCurrent = stringManipulate.IndexOf(' ') + 1;//find the first space 
            var transactionTime = stringManipulate.Substring(startIndex, endIndexCurrent); //and copy that as amount to variable
            stringManipulate = stringManipulate.Remove(startIndex, endIndexCurrent);//delete the copied amount from this string
            
            return resultStoreData;
        }
        public SMSMessageStoreData ProcessCustomerDeposit(SMSMessageStore rawMessage)
        {
            var resultStoreData = new SMSMessageStoreData();
            //OAO4KVPDQ4 Confirmed. On 24/1/20 at 8:26 AM Take Ksh1,100.00 cash from PASCALENE W MAINA Your M-PESA float balance is Ksh27,197.00.

            return resultStoreData;
        }
        public SMSMessageStoreData ProcessCustomerWithdraw(SMSMessageStore rawMessage)
        {
            var resultStoreData = new SMSMessageStoreData();
            //OAN3KNQZC7 Confirmed. on 23/1/20 at 7:51 PM Give Ksh1,530.00 to JAPHET MAINA MBOGO 0720435807. New M-PESA float balance is Ksh28,297.00

            return resultStoreData;
        }
        public SMSMessageStoreData Process(SMSMessageStore rawMessage)
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
                return this.ProcessPurchaseAirtime(rawMessage);
            else if (rawMessage.TextMessage.ToLower().Contains("Give".ToLower()))
                return this.ProcessCustomerWithdraw(rawMessage);
            else if (rawMessage.TextMessage.ToLower().Contains("Take".ToLower()))
                return this.ProcessCustomerWithdraw(rawMessage);
            throw new NotImplementedException("Sorry supplied message is not supported");
        }

        public SMSMessageStoreData ProcessAndSave(SMSMessageStore rawMessage)
        {
            var proccessedSMS = this.Process(rawMessage);
            DateTime foo = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            var taskCreate = App.Database.DatabaseConnection.InsertAsync(new SMSMessageStoreData
            {
                UnixTimeStamp = unixTime,
                SMSMessageStoreId = rawMessage.Id,
                TransactionId = proccessedSMS.TransactionId,
                TransactionTime = proccessedSMS.TransactionTime,
                AmountIn = proccessedSMS.AmountIn,
                AmountOut = proccessedSMS.AmountOut,
                ClientIdentifier = proccessedSMS.ClientIdentifier,
                TransactionSynced = false,
                TransactionType = proccessedSMS.TransactionType,
            });
            taskCreate.Wait();

            var taskWait = App.Database.DatabaseConnection.Table<SMSMessageStoreData>()
                .Where(r => r.Id == taskCreate.Result)
                .FirstOrDefaultAsync();
            taskWait.Wait();
            return taskWait.Result;
        }

        public bool Support(string senderId)
        {
            if (string.IsNullOrEmpty(senderId))
                return false;
            return senderId.ToLower() == "MPESA".ToLower();
        }
    }
}
