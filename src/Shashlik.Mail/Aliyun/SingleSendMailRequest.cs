using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Transform;
using Aliyun.Acs.Core.Utils;

namespace Guc.Mail.Aliyun
{
    public class SingleSendMailRequest : RpcAcsRequest<SingleSendMailResponse>
    {
        public SingleSendMailRequest()
            : base("Dm", "2015-11-23", "SingleSendMail", "dm", "openAPI")
        {
        }

		private string htmlBody;

		private string fromAlias;

		private long? resourceOwnerId;

		private string resourceOwnerAccount;

		private string toAddress;

		private string clickTrace;

		private string subject;

		private long? ownerId;

		private string replyAddressAlias;

		private string accessKeyId;

		private string tagName;

		private string accountName;

		private bool? replyToAddress;

		private string replyAddress;

		private string action;

		private int? addressType;

		private string textBody;

		public string HtmlBody
		{
			get => htmlBody;
            set	
			{
				htmlBody = value;
				DictionaryUtil.Add(QueryParameters, "HtmlBody", value);
			}
		}

		public string FromAlias
		{
			get => fromAlias;
            set	
			{
				fromAlias = value;
				DictionaryUtil.Add(QueryParameters, "FromAlias", value);
			}
		}

		public long? ResourceOwnerId
		{
			get => resourceOwnerId;
            set	
			{
				resourceOwnerId = value;
				DictionaryUtil.Add(QueryParameters, "ResourceOwnerId", value.ToString());
			}
		}

		public string ResourceOwnerAccount
		{
			get => resourceOwnerAccount;
            set	
			{
				resourceOwnerAccount = value;
				DictionaryUtil.Add(QueryParameters, "ResourceOwnerAccount", value);
			}
		}

		public string ToAddress
		{
			get => toAddress;
            set	
			{
				toAddress = value;
				DictionaryUtil.Add(QueryParameters, "ToAddress", value);
			}
		}

		public string ClickTrace
		{
			get => clickTrace;
            set	
			{
				clickTrace = value;
				DictionaryUtil.Add(QueryParameters, "ClickTrace", value);
			}
		}

		public string Subject
		{
			get => subject;
            set	
			{
				subject = value;
				DictionaryUtil.Add(QueryParameters, "Subject", value);
			}
		}

		public long? OwnerId
		{
			get => ownerId;
            set	
			{
				ownerId = value;
				DictionaryUtil.Add(QueryParameters, "OwnerId", value.ToString());
			}
		}

		public string ReplyAddressAlias
		{
			get => replyAddressAlias;
            set	
			{
				replyAddressAlias = value;
				DictionaryUtil.Add(QueryParameters, "ReplyAddressAlias", value);
			}
		}

		public string AccessKeyId
		{
			get => accessKeyId;
            set	
			{
				accessKeyId = value;
				DictionaryUtil.Add(QueryParameters, "AccessKeyId", value);
			}
		}

		public string TagName
		{
			get => tagName;
            set	
			{
				tagName = value;
				DictionaryUtil.Add(QueryParameters, "TagName", value);
			}
		}

		public string AccountName
		{
			get => accountName;
            set	
			{
				accountName = value;
				DictionaryUtil.Add(QueryParameters, "AccountName", value);
			}
		}

		public bool? ReplyToAddress
		{
			get => replyToAddress;
            set	
			{
				replyToAddress = value;
				DictionaryUtil.Add(QueryParameters, "ReplyToAddress", value.ToString());
			}
		}

		public string ReplyAddress
		{
			get => replyAddress;
            set	
			{
				replyAddress = value;
				DictionaryUtil.Add(QueryParameters, "ReplyAddress", value);
			}
		}

		public string Action
		{
			get => action;
            set	
			{
				action = value;
				DictionaryUtil.Add(QueryParameters, "Action", value);
			}
		}

		public int? AddressType
		{
			get => addressType;
            set	
			{
				addressType = value;
				DictionaryUtil.Add(QueryParameters, "AddressType", value.ToString());
			}
		}

		public string TextBody
		{
			get => textBody;
            set	
			{
				textBody = value;
				DictionaryUtil.Add(QueryParameters, "TextBody", value);
			}
		}

        public override SingleSendMailResponse GetResponse(UnmarshallerContext unmarshallerContext)
        {
            return SingleSendMailResponseUnmarshaller.Unmarshall(unmarshallerContext);
        }
    }
}
