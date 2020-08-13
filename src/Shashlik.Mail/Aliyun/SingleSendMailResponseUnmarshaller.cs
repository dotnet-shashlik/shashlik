using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.Acs.Core.Transform;

namespace Guc.Mail.Aliyun
{
    public class SingleSendMailResponseUnmarshaller
    {
        public static SingleSendMailResponse Unmarshall(UnmarshallerContext context)
        {
            SingleSendMailResponse singleSendMailResponse = new SingleSendMailResponse();

            singleSendMailResponse.HttpResponse = context.HttpResponse;
            singleSendMailResponse.RequestId = context.StringValue("SingleSendMail.RequestId");
            singleSendMailResponse.EnvId = context.StringValue("SingleSendMail.EnvId");
        
            return singleSendMailResponse;
        }
    }
}
