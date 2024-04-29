using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Data
{
    public class Purchase
    {
        public int ID { get; set; }
        public long? CustomerID { get;set; }
        public string CustomerName { get; set; }
        public string Goods { get; set; }
        public string Date { get; set; } = DateTime.Now.ToShortDateString() + " Time:" + DateTime.Now.TimeOfDay.ToString();
        public ushort State { get; set; } = 0;  //0 - Should be done, 1 - done
        public Purchase(int ID,long? CustomerID, string? customerName, string Goods, string Date, ushort State)
        {
            this.ID = ID;
            this.Goods = Goods;
            this.Date = Date;
            this.State = State;
            this.CustomerID = CustomerID == null ? -1 : CustomerID;
            CustomerName = customerName == null ? "No name" : customerName;
        }

        
        public string GetStringState()
        {
            string state = string.Empty;
            switch (State)
            {
                case 0: state = "⏱"; break;
                case 1: state = "▶️"; break;
                case 2: state = "✅"; break;
            }
            return state;
        }
    }
}
