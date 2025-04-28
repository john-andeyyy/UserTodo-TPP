using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Models.Requests
{
    public class GenericAPIRequest
    {
        public string ApiKey { get; set; }
        public string SessionId { get; set; }
        public string MethodName { get; set; }
        public string IpAddress { get; set; }

        public string ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramDescription { get; set; }
        public string BasicUser { get; set; }
        public string BasicPasswowrd { get; set; }

        //generic for profiles
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string TransType {get; set; }
        public string TransTypeId {get; set; }
        public string IsPrimary {get; set; }

        public string Content { get; set; }
        public string Subject { get; set; }
        public string Footer { get; set; }
        public string Header { get; set; }

        public string NotificationId { get; set; }
        
        //SendSMS
        public string TemplateId { get; set; }
        public string MobileNumber { get; set; }
        public string TemplateCode { get; set; }
        public string DateSent { get; set; }

        //SendEmail
        public string EmailReceiver { get; set; }

        //Tags
        public string TransactionTypeCode { get; set; }

        //Vendor
        public string SMSEndPoint { get; set; }
        public string SMSUsername { get; set; }
        public string SMSPassword { get; set; }

        
        public string SendKey { get; set; }
        public string SendTemplateKey { get; set; }
        public string SenderEmail { get; set; }

        public bool IsActive { get; set; }

        //provide details
        public string ProviderId { get; set; }
        public string ProviderTypeId { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ProviderDescription { get; set; }
        public List<Dictionary<string, string>> Credentials { get; set; }
        public string CredsJson { get; set; }


        //provider type
        public string ProviderTypeCode { get; set; }
        public string ProviderTypeName { get; set; }
        public string ProviderTypeDescription { get; set; }

        public string CredentialId { get; set; }
        public string ApiGuid { get; set; }

    }

    public class GenericAPIRequestV2
    {
        public string apiKey { get; set; }
        public string sessionId { get; set; }
        public string methodName { get; set; }
        public string ipAddress { get; set; }

        public string programId { get; set; }
        public string programName { get; set; }
        public string programDescription { get; set; }
        public string basicUser { get; set; }
        public string basicPasswowrd { get; set; }

        //generic for profiles
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string transType { get; set; }
        public string transTypeId { get; set; }
        public string isPrimary { get; set; }

        public string content { get; set; }
        public string subject { get; set; }
        public string footer { get; set; }
        public string header { get; set; }

        public string notificationId { get; set; }

        //SendSMS
        public string templateId { get; set; }
        public string mobileNumber { get; set; }
        public string templateCode { get; set; }
        public string dateSent { get; set; }

        //SendEmail
        public string emailReceiver { get; set; }

        //Tags
        public string transactionTypeCode { get; set; }

        //Vendor
        public string smsEndpoint { get; set; }
        public string smsUsername { get; set; }
        public string smsPassword { get; set; }


        public string sendKey { get; set; }
        public string sendTemplateKey { get; set; }
        public string senderEmail { get; set; }

        public bool isActive { get; set; }

        //provide details
        public string providerId { get; set; }
        public string providerTypeId { get; set; }
        public string providerCode { get; set; }
        public string providerName { get; set; }
        public string providerDescription { get; set; }
        public List<Dictionary<string, string>> credentials { get; set; }
        public string credsJson { get; set; }


        //provider type
        public string providerTypeCode { get; set; }
        public string providerTypeName { get; set; }
        public string providerTypeDescription { get; set; }

        public string credentialId { get; set; }
        public string apiGuid { get; set; }

    }
}
