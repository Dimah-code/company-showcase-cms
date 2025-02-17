using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PortoWeb.Models;

namespace PortoWeb.Services
{
    public class MessageService
    {
        public void DeleteOldMessages()
        {
            using (var context = new PortoDB1())
            {
                var sevenDaysAgo = DateTime.Now.AddDays(-7);
                var oldMessages = context.Table_Messages.Where(m => m.CreatedAt < sevenDaysAgo).ToList();

                if (oldMessages.Any())
                {
                    context.Table_Messages.RemoveRange(oldMessages);
                    context.SaveChanges();
                }
            }
        }
    }

}