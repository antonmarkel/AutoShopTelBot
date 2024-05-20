using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Items
{
    public class Picture
    {
        public int ID {  get; set; }    
        public string FileIdentifier { get; set; }  
        public string Name { get; set; }
        public Picture(string fileIdentifier,string Name)
        {
            FileIdentifier = fileIdentifier;   
            this.Name = Name;
        }
    }
}
