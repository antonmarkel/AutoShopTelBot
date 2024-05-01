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
        public decimal Cost { get; set; }   
        public long? CustomerID { get;set; }
        public string CustomerName { get; set; }
        public List<string> Goods { get; set; }
        public string Date { get; set; } = DateTime.Now.ToShortDateString() + " Time:" + DateTime.Now.TimeOfDay.ToString();
        public ushort State { get; set; } = 0;  //0 - Should be done, 1 - done
        public string Data { get; set; } = string.Empty;
        public Purchase(int ID,decimal Cost,long? CustomerID, string? customerName, List<string> Goods, string Date, ushort State, string data)
        {
            this.ID = ID;
            this.Cost = Cost;
            this.Goods = Goods;
            this.Date = Date;
            this.State = State;
            this.CustomerID = CustomerID == null ? -1 : CustomerID;
            CustomerName = customerName == null ? "No name" : customerName;
            this.Data = data;
        }

        public List<string> GetCategories()
        {
            List<string> ans = new List<string>();
            foreach(var good in Goods)
            {
                var item = Items.All.FirstOrDefault(v => v.Identifier == good);
                if(!ans.Contains(item.Category))ans.Add(item.Category);
            }
            return ans;
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
