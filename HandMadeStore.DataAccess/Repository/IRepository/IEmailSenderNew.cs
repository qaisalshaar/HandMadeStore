using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository.IRepository
{
    public interface IEmailSenderNew


    {

       Task SendEmailAsync(string email,string subject,string message);
    }
}
