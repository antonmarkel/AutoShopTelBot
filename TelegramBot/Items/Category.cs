using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Items
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryType { get; set; }
        public int PictureID { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public Category(string Name,string Description,string CategoryType,int PictureID) {
 
            this.Name = Name;
            this.Description = Description;
            this.CategoryType = CategoryType;
            this.PictureID = PictureID;
        }
        public Category() { }
    }
}
