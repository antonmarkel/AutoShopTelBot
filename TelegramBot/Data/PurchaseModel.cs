using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Data
{
    public class PurchaseModel
    {
        public int ID { get; set; }
        public decimal Cost { get; set; }
        public long? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string JsonGoods { get; set; }
        public string Date { get; set; } = DateTime.Now.ToShortDateString() + " Time:" + DateTime.Now.TimeOfDay.ToString();
        public ushort State { get; set; } = 0;  //0 - Should be done, 1 - done
        public string Data { get; set; } = string.Empty;
        public string? PictFileID { get; set; } = null;

        public PurchaseModel(int ID, decimal Cost, long? CustomerID, string? customerName, string JsonGoods, string Date, ushort State, string data, string PictFileID)
        {
            this.ID = ID;
            this.Cost = Cost;
            this.JsonGoods = JsonGoods;
            this.Date = Date;
            this.State = State;
            this.CustomerID = CustomerID == null ? -1 : CustomerID;
            CustomerName = customerName == null ? "No name" : customerName;
            this.Data = data;
            this.PictFileID = PictFileID;
        }
    }
}
