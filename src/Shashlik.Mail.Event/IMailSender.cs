using Guc.EventBus;
using System;
using System.Collections.Generic;
using System.Text;
using Guc.Kernel.Dependency;

namespace Guc.Mail
{
    /// <summary>
    /// 邮件发送
    /// </summary>
    public interface IMailSender
    {
        /// <summary>
        /// 发送普通HTML邮件
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        void Send(string address, string subject, string content);
    }

    public class DefaultMailSender : IMailSender, Guc.Kernel.Dependency.ISingleton
    {
        private IEventPublisher EventPublisher { get; }
        public DefaultMailSender(IEventPublisher eventPublisher)
        {
            EventPublisher = eventPublisher;
        }
        public void Send(string address, string subject, string content)
        {
            EventPublisher.Publish(new SendMailEvent()
            {
                Address = address,
                Subject = subject,
                Content = content
            });
        }
    }


}
