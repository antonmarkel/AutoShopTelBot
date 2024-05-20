using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Items
{
    public class Item
    {
        public int ID { get; set; } 
        public string Name { get; set; }       
        public int CategoryID { get; set; }
        public int PictureID { get; set; } = -1;
        public bool IsHot { get;set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public Item(string Name, int CategoryID,int PictureID,bool isHot,decimal Price,string Description) {
            this.Name = Name;
            this.CategoryID = CategoryID;
            this.PictureID = PictureID;
            this.Price = Price;
            this.Description = Description;
            this.IsHot = isHot;
        }

     }
}
