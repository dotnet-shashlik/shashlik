using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Options;

namespace Shashlik.Sms;

[Singleton]
[ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.Enable), true, DefaultValue = true)]
public class DefaultSmsSender : ISmsSender
{
    public DefaultSmsSender(IOptionsMonitor<SmsOptions> options, IEnumerable<ISmsProvider> smsProviders)
    {
        Options = options;
        SmsProviders = smsProviders;
    }

    private IOptionsMonitor<SmsOptions> Options { get; }
    private IEnumerable<ISmsProvider> SmsProviders { get; }

    private ISmsProvider Get()
    {
        var provider = SmsProviders.SingleOrDefault(r => r.ProviderName == Options.CurrentValue.Provider);
        if (provider is null)
            throw new OptionsValidationException(nameof(SmsOptions.Provider), typeof(SmsOptions), new[] {"invalid sms provider."});
        return provider;
    }

    public void SendCheck(string phone, string subject, params string[] args)
    {
        Get().SendCheck(phone, subject, args);
    }

    public Task<string> SendWithCheckAsync(string phone, string subject, params string[] args)
    {
        return Get().SendWithCheckAsync(phone, subject, args);
    }

    public Task<string> SendAsync(IEnumerable<string> phones, string subject, params string[] args)
    {
        return Get().SendAsync(phones, subject, args);
    }
}