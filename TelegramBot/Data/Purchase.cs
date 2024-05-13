using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Data
{
    public class Purchase
    {
        public int Identifier { get; set; }
        public int IndexNumber { get; set; }
        public ulong Cost { get; set; }   
        public long? CustomerID { get;set; }
        public string CustomerName { get; set; }
        public string PaymentSystem { get;set; }
        public List<string> Goods { get; set; }
        public string Date { get; set; } = DateTime.Now.ToShortDateString() + " Time:" + DateTime.Now.TimeOfDay.ToString();
        public ushort State { get; set; } = 0; 
        public string Data { get; set; } = string.Empty;

        public InputFile Pict { get; set; } = null;
        private string PictFileID { get; set; } 

        public Purchase(PurchaseModel dataModel)
        {
            Identifier = dataModel.Identifier;
            Cost = dataModel.Cost;
            CustomerID = dataModel.CustomerID;
            CustomerName = dataModel.CustomerName;
            PaymentSystem = dataModel.PaymentSystem;
            Goods = JsonSerializer.Deserialize<List<string>>(dataModel.JsonGoods);
            Date = dataModel.Date;
            State = dataModel.State;
            Data = dataModel.Data;
            PictFileID = dataModel.PictFileID;
            IndexNumber = dataModel.IndexNumber;
            if(dataModel.PictFileID != null)Pict = InputFile.FromFileId(dataModel.PictFileID);
        }
        public Purchase(int identifier,int indexNumber, ulong cost, long? customerID, string customerName,string paymentSystem, List<string> goods, string date, ushort state, string data, InputFile pict)
        {
            Identifier = identifier;
            IndexNumber = indexNumber;
            Cost = cost;
            CustomerID = customerID;
            CustomerName = customerName;
            PaymentSystem = paymentSystem;
            Goods = goods;
            Date = date;
            State = state;
            Data = data;
            Pict = pict;

        }

        public PurchaseModel ToDataModel()
        {
            return new PurchaseModel(Identifier,IndexNumber, Cost, CustomerID, CustomerName,PaymentSystem, JsonSerializer.Serialize(Goods), Date, State, Data, PictFileID);
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
