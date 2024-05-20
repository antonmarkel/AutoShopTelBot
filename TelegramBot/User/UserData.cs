using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TelegramBot.StateComputing;

namespace TelegramBot.User
{
    public class UserData
    {
        public int Id { get; set; }
        public long ChatId { get;set; }

        public short ChatStateData { get; set; }    
        [NotMapped]
        public ChatStates ChatState { get;set; }
        public int CurrentMessageId { get; set; }

        [NotMapped]
        public Dictionary<string,object> TempData { get; set; } = new Dictionary<string,object>();

        public UserData(long ChatId,short chatStateData,int CurrentMessageId) {
            this.ChatId = ChatId;
            this.ChatState = (ChatStates)chatStateData;
            this.ChatStateData = chatStateData;
            this.CurrentMessageId = CurrentMessageId;
        }
    }
}
