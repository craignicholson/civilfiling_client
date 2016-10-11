using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;

namespace CivilFilingClient
{
    /// <summary>
    ///  OASIS Web Services Security UsernameToken Profile 1.1
    ///  SOAP Message Security specification
    ///  https://www.oasis-open.org/committees/download.php/13392/wss-v1.1-spec-pr-UsernameTokenProfile-01.htm
    ///  
    /// This class will generate a soap header 
    ///     <Security xmlns="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
    ///       <wsse:UsernameToken xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
    ///         <wsse:UsernameToken Id="unt_20" xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
    ///         <wsse:Username>888888005</wsse:Username>
    ///         <wsse:Password Type = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">P@ssword</wsse:Password>
    ///       </wsse:UsernameToken>
    ///     </Security>
    /// </summary>
    /// Example C# 
    /// client = new CivilFilingServiceReference.CivilFilingWSClient("CivilFilingWSPort", address);
    /// using (new OperationContextScope(client.InnerChannel))
    /// {
    ///     OperationContext.Current.OutgoingMessageHeaders.Add(new SecurityHeader("unt_20", "888888005", "P@ssword"));
    ///     
    ///     //Send the Request with in the using statement, if you don't the Soap Header is not sent
    ///     filingReponse = client.submitCivilFiling(filingRequest);
    /// }

    public class SecurityHeader : MessageHeader
    {
        private readonly UsernameToken _usernameToken;

        public SecurityHeader(string id, string username, string password)
        {
            _usernameToken = new UsernameToken(id, username, password);
        }

        public override string Name
        {
            get { return "Security"; }
        }

        public override string Namespace
        {
            get { return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            XmlSerializer serializer = new XmlSerializer(typeof(UsernameToken));
            serializer.Serialize(writer, _usernameToken,ns);
        }
    }


    [XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public class UsernameToken
    {
        public UsernameToken()
        {
        }

        public UsernameToken(string id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = new Password() { Value = password };
        }

        //[XmlAttribute(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        [XmlAttribute]
        public string Id { get; set; }

        [XmlElement]
        public string Username { get; set; }

        [XmlElement]
        public Password Password { get; set; }
    }

    public class Password
    {
        public Password()
        {
            Type = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText";
        }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}