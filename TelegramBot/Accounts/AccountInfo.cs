using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Accounts
{
    public class AccountInfo
    {
        public decimal Balance { get;private set; }
        public string TelegramAccount { get; private set; }

        public void AddBalance(decimal amount)
        {
            Balance += amount;
        }
        public void RemoveBalance(decimal amount)
        {
            Balance -= amount;
        }  

    }
}
