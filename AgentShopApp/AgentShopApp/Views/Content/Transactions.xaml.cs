using AgentShopApp.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgentShopApp.Views.Content
{
    public partial class Transactions : ContentPage
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
        public Transactions()
        {
            InitializeComponent();
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var binding = ((Editor)sender).GetBindingExpression(Editor.TextProperty);
            //binding.UpdateSource();
        }
    }
}
