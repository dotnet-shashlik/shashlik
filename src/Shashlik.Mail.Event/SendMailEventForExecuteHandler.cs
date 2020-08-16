using System.Threading.Tasks;

namespace Shashlik.Mail
{
    public class SendMailEventForExecuteHandler : Shashlik.EventBus.IEventHandler<SendMailEvent>
    {
        private IMail Mail { get; }
        public SendMailEventForExecuteHandler(IMail mail)
        {
            Mail = mail;
        }

        public Task Execute(SendMailEvent @event)
        {
            Mail.Send(@event.Address, @event.Subject, @event.Content);
            return Task.CompletedTask;
        }
    }
}
