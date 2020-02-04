using AgentShopApp.Data.Model;
using AgentShopApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using AgentShopApp.SMSProcessor;
using AgentShopApp.Dependency.SMS;

namespace AgentShopApp.ViewModel
{

    public class MainPageViewModel : INotifyPropertyChanged
    {

        List<TransactionType> transactionTypes;
        public IEnumerable<TransactionType> TransactionTypes
        {
            get
            {
                if (transactionTypes == null)
                {
                    var resultListTask = App.Database.DatabaseConnection.Table<TransactionType>().ToListAsync();
                    resultListTask.Wait();
                    var finalResult = new List<TransactionType> { new Data.Model.TransactionType { Id = 0, Name = "View All" } };
                    finalResult.AddRange(resultListTask.Result);
                    transactionTypes = finalResult;
                }
                return transactionTypes;
            }
        }

        DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndDate)));
            }
        }

        DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartDate)));
            }
        }
        TransactionType _transactionType;
        public TransactionType TransactionType
        {
            get
            {
                return _transactionType;
            }
            set
            {
                _transactionType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TransactionType)));
            }
        }
        string _phoneNumber;
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneNumber)));
            }
        }

        string _customerName;
        public string CustomerName
        {
            get
            {
                return _customerName;
            }
            set
            {
                _customerName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomerName)));
            }
        }
        string _transactionId;
        public string TransactionId
        {
            get
            {
                return _transactionId;
            }
            set
            {
                _transactionId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TransactionId)));
            }
        }

        IEnumerable<SMSMessageStoreData> _sMSMessageStoreData;
        public IEnumerable<SMSMessageStoreData> SMSMessageStoreData
        {
            get => _sMSMessageStoreData;

            set
            {
                _sMSMessageStoreData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SMSMessageStoreData)));
            }
        }

        public string SyncCommandText
        {
            get => "Sync";
        }

        public string RefreshCommandText
        {
            get => "Refresh";
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            this._sMSMessageStoreData = new HashSet<SMSMessageStoreData>();
            RefreshCommand = new Command(this.OnRefreshCommand);
            SyncCommand = new Command(this.OnSyncCommand);

            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.Now;
        }

        private void OnSyncCommand(object obj)
        {
            try
            {
                //start the service using an intent
                var modelFilter = new SMSReaderFilterModel
                {
                    EndDate = this.EndDate.AddDays(1),
                    StartDate = this.StartDate,
                    SenderId = SMSSaverRepository.InterceptedSenders
                };

                var service = DependencyService.Get<IProcessSMS>();
                service.ScheduleJob(modelFilter);
            }
            catch(Exception ex)
            {
                int y = 0;
            }
        }

        public async void OnRefreshCommand(object obj)
        {
            //use the supplied parametres to load the data
            try
            {
                var whereClause = string.Empty;
                var listParams = new List<object>();
                if (string.IsNullOrEmpty(this.CustomerName) == false)
                {
                    whereClause = string.Format(" {0} and cd.ClientName = ? ", whereClause);
                    listParams.Add(this.CustomerName);
                }
                if (string.IsNullOrEmpty(this.PhoneNumber) == false)
                {
                    whereClause = string.Format(" {0} and cd.ClientPhone = ? ", whereClause);
                    listParams.Add(this.PhoneNumber);
                }
                if (string.IsNullOrEmpty(this.TransactionId) == false)
                {
                    whereClause = string.Format(" {0} and st.TransactionId = ? ", whereClause);
                    listParams.Add(this.TransactionId);
                }

                if (this.TransactionType != null
                    && this.TransactionType.Id > 0)
                {
                    whereClause = string.Format(" {0} and st.TransactionTypeId = ? ", whereClause);
                    listParams.Add(this.TransactionType.Id);
                }
                if (this.StartDate != null)
                {
                    //whereClause = string.Format(" {0} and st.TransactionTime >= date(?) ", whereClause);
                    //listParams.Add(this.StartDate);
                }

                if (this.EndDate != null)
                {
                    //date(period2.DateEnd, '+1 day')
                    //whereClause = string.Format(" {0} and st.TransactionTime < date(?, '+1 day')  ", whereClause);
                    //listParams.Add(this.EndDate);
                }
                var resultData = await App.Database.DatabaseConnection
                    .QueryAsync<Mapper>(string.Format(@"
                SELECT 
                    st.Id,
                    st.UnixTimeStamp,
                    st.SMSMessageStoreId,
                    st.TransactionId,
                    st.TransactionTime,
                    st.Amount,
                    st.ClientDataId,
                    st.TransactionSynced,
                    st.TransactionTypeId,
                    st.Narration,
                    ms.TextMessage,
                    cd.ClientName,
                    cd.ClientPhone,
                    tt.Name as TransactionType
                FROM
                    SMSMessageStoreData AS st
                        INNER JOIN
                    ClientData AS cd ON cd.Id = st.ClientDataId
                        INNER JOIN
                    SMSMessageStore ms ON ms.Id = st.SMSMessageStoreId
                         INNER JOIN
                    TransactionType tt ON tt.Id = st.TransactionTypeId
                WHERE
                    1 {0} ;", whereClause), listParams.ToArray());

                this.SMSMessageStoreData = resultData
                    .Select(r => new Data.Model.SMSMessageStoreData
                    {
                        Id = r.Id,
                        Amount = r.Amount,
                        ClientData = new ClientData
                        {
                            Id = r.ClientDataId,
                            ClientName = r.ClientName,
                            ClientPhone = r.ClientPhone,
                        },
                        ClientDataId = r.ClientDataId,
                        Narration = r.Narration,
                        SMSMessageStore = new SMSMessageStore
                        {
                            Id = r.Id,
                            TextMessage = r.TextMessage
                        },
                        SMSMessageStoreId = r.SMSMessageStoreId,
                        TransactionId = r.TransactionId,
                        TransactionSynced = r.TransactionSynced,
                        TransactionTime = r.TransactionTime,
                        TransactionType = new TransactionType
                        {
                            Id = r.TransactionTypeId,
                            Name = r.TransactionType,
                        },
                        TransactionTypeId = r.TransactionTypeId,
                        UnixTimeStamp = r.UnixTimeStamp
                    });
            }
            catch (Exception ex)
            {
                int y = 0;
            }
        }

        public Command RefreshCommand { get; }

        public Command SyncCommand { get; }

        class Mapper
        {
            public long Id { get; set; }
            public long UnixTimeStamp { get; set; }
            public long SMSMessageStoreId { get; set; }
            public string TransactionId { get; set; }
            public DateTime TransactionTime { get; set; }
            public double Amount { get; set; }
            public int ClientDataId { get; set; }
            public bool TransactionSynced { get; set; }
            public int TransactionTypeId { get; set; }
            public string Narration { get; set; }
            public string TextMessage { get; set; }
            public string ClientName { get; set; }
            public string ClientPhone { get; set; }
            public string TransactionType { get; set; }
        }


    }
}
