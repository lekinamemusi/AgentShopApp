using AgentShopApp.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgentShopApp
{
    public partial class MainPage : ContentPage
    {
        public IList<TransactionType> TransactionTypes
        {
            get
            {
                var result = App.Database.DatabaseConnection.Table<TransactionType>()
                    .ToListAsync();
                result.Wait();
                return result.Result;
            }
        }
        public MainPage()
        {
            InitializeComponent();
        }
    }
}
