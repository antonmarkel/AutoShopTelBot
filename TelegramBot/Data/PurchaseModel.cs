using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Data
{
    public class PurchaseModel
    {
        [Key]
        public int ID { get; set; } = 0;
        public int Identifier { get; set; }
        public ulong Cost { get; set; }
        public long? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string JsonGoods { get; set; }
        public string PaymentSystem { get; set; }
        public string Date { get; set; } = DateTime.Now.ToShortDateString() + " Time:" + DateTime.Now.TimeOfDay.ToString();
        public ushort State { get; set; } = 0;  //0 - Should be done, 1 - done
        public string Data { get; set; } = string.Empty;
        public string? PictFileID { get; set; } = null;

        public Purchase ToModel()
        {
            return new Purchase(this);
        }
        public PurchaseModel(int ID, ulong Cost, long? CustomerID, string? customerName, string PaymentSystem, string JsonGoods, string Date, ushort State, string data, string PictFileID)
        {
            this.Identifier = ID;
            this.Cost = Cost;
            this.JsonGoods = JsonGoods;
            this.Date = Date;
            this.State = State;
            this.CustomerID = CustomerID == null ? -1 : CustomerID;
            this.PaymentSystem = PaymentSystem;
            CustomerName = customerName == null ? "No name" : customerName;
            this.Data = data;
            this.PictFileID = PictFileID;
        }
    }
}
