using System.Threading.Tasks;

namespace Guc.Mail
{
    public class SendMailEventForExecuteHandler : Guc.EventBus.IEventHandler<SendMailEvent>
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
