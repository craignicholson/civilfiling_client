# Civil Filing Client

Requirements

------------

Law firm in NJ uses a system to interact with NJ Legal system.
NJ Courts moving to E-Filing over SOAP.

A windows form app that loads XML files and corresponding pdf attachments 
from a directory and submits them over the SOAP service.  

Save output of SOAP service into a text file in the same directory.

                OperationContext.Current.OutgoingMessageHeaders.Add(new SecurityHeader("feinsuch", "F00000495", "Feinsuch#1"));
                OperationContext.Current.OutgoingMessageHeaders.Add(new SecurityHeader("feinsuch", "888888029", "P@ssword"));


Hi Dan,

Here is the updated doc from our IT I spoke about. They updated the URL to connect to the webservice. Test: https://portal.njcourts.gov/webe19/ecourtsweb/pages/home/home.faces

 

Also here is Vijaya’s contact info it you have technical questions Vijaya.thatipallyl1@njcourts.gov or at 609-815-2900 ext. 35735.


Good afternoon:

 

In the past month or so, Chris Baghsarian from our office had reached out to you regarding the current JEFIS Bulk Noticing process. This was done due to the fact that our IT department here at the Judiciary was looking to move away from the current delivery method, and replace it with a more modern process. With that in mind, the current plan is to provide this notice data via webservice. Subsequently, a new webservice method has been deployed to our training environment for testing purposes, and updated documentation is attached to this email for reference. At the moment, no data/notices are currently available to retrieve, so the purpose of this initial testing would just be to ensure that you can call the new method successfully. Instead of receiving the notices, you would just receive a hard coded response at this time. In the coming weeks, we will be introducing more production based data into the training environment, and we will expand the scope of testing accordingly and provide updates along the way.

 

Throughout this process, if you run into any issues, please feel free to contact Chris or myself, and if you have any technical questions, please contact Vijaya Thatipally in our IT department directly at Vijaya.thatipallyl1@njcourts.gov or at 609-815-2900 ext. 35735.

 

Thank you.

 

Michael G. Levins

Administrative Office of the Courts

Automated Trial Court Services Unit

Hi Dan,

Are you able to connect to the new webservice? If so how is it going, we are anticipating having Data in the training environment this week. Thanks.

 

Chris Baghsarian

Automated Trial Court Services Unit

609-815-2900 ext 51878


ex.Message
"Error creating a reader for the MTOM message"
ex.InnerException
{"MTOM messages must have type 'application/xop+xml'."}
    Data: {System.Collections.ListDictionaryInternal}
    HResult: -2146232000
    HelpLink: null
    InnerException: null
    LineNumber: 0
    LinePosition: 0
    Message: "MTOM messages must have type 'application/xop+xml'."
    Source: "System.Runtime.Serialization"
    SourceUri: null
    StackTrace: "   at System.Xml.XmlMtomReader.ReadMessageContentTypeHeader(ContentTypeHeader header, String& boundary, String& start, String& startInfo)\r\n   at System.Xml.XmlMtomReader.Initialize(Stream stream, String contentType, XmlDictionaryReaderQuotas quotas, Int32 maxBufferSize)\r\n   at System.Xml.XmlMtomReader.SetInput(Stream stream, Encoding[] encodings, String contentType, XmlDictionaryReaderQuotas quotas, Int32 maxBufferSize, OnXmlDictionaryReaderClose onClose)\r\n   at System.ServiceModel.Channels.MtomMessageEncoder.MtomBufferedMessageData.TakeXmlReader()"
    TargetSite: {Void ReadMessageContentTypeHeader(System.Xml.ContentTypeHeader, System.String ByRef, System.String ByRef, System.String ByRef)}







ex.Message
"The maximum message size quota for incoming messages (65536) has been exceeded. To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element."

      <basicHttpBinding>
        <binding name="CivilFilingWSPortBinding" sendTimeout="00:03:00" maxBufferSize="999999" maxReceivedMessageSize="999999">
          <security mode="Transport">
            <transport clientCredentialType="None" />
          </security>
        </binding>
        <binding name="CivilFilingWSService_CivilFilingWSPort" sendTimeout="00:03:00"
          messageEncoding="Mtom" maxBufferSize="999999" maxReceivedMessageSize="999999">
          <security mode="Transport">
            <transport clientCredentialType="None" />
          </security>
        </binding>
      </basicHttpBinding>


FIX - https://stackoverflow.com/questions/9967053/where-to-put-maxreceivedmessagesize-property-in-wcf-services-web-config-file

        <binding name="CivilFilingWSPortBinding" sendTimeout="00:03:00" maxBufferSize="999999" maxReceivedMessageSize="999999">
          <security mode="Transport">
            <transport clientCredentialType="None" />
          </security>
        </binding>
        <binding name="CivilFilingWSService_CivilFilingWSPort" sendTimeout="00:03:00"
          messageEncoding="Mtom" maxBufferSize="999999" maxReceivedMessageSize="999999">
          <security mode="Transport">
            <transport clientCredentialType="None" />
          </security>
        </binding>





Features

------------

* Attach multiple xml and pdf files for batch processing.
* Switch Production and Test EndPoints using the Settings on the menu.
* Application expects one packet per xml file.
* Each xml file processed will output results into a separate file.
* The response from the web service will be written to the same directory the xml and pdf originate.
* Successful requests move the xml and pdf files into an archive folder.
* Daily log files located in the application folder.
* Command Line Interface for automation.

Command Line Interface Example

------------
One can execute the application from the command line.  Here is an example  you can run from windows command prompt.

```cmd

C:\> cd "C:\Users\CivilFilingClient\"
C:\Users\CivilFilingClient> CivilFilingClient.exe "888888005" "P@ssword" "https://dptng.njcourts.gov:2045/civilFilingWS_t" "C:\Files\TestCorp2Corp_MissingBranchID.xml" 

C:\Users\CivilFilingClient> CivilFilingClient.exe "888888005" "P@ssword" "https://dptng.njcourts.gov:2045/civilFilingWS_t" "C:\Files\TestIndivid2Individ.xml" 

```

It does require the pdf to be in the same folder as the xml file.

The parameters to pass in are the following:

* Username
* Password
* EndPoint {testing or production}
* xml full file Path

There is no output back to the command line.  Review the log files for the results.

TODO:  Also include the pdf file path.  Currently the application will expect the pdf to be in the same directory as
the xml file.

## Windows Batch File Example

What you can do is create a windows batch file which is a text file with .bat extension.

Inside of the batch file you can have several of the above commands:

2016_10_31_eCourtSuits.bat or eCourtSuits.bat
(We can timestamp and run this manaully or name the batch file the same name every day)

```cmd

C:\Users\CivilFilingClient> CivilFilingClient.exe "888888005" "P@ssword" "https://dptng.njcourts.gov:2045/civilFilingWS_t" "C:\Files\File1.xml" 
C:\Users\CivilFilingClient> CivilFilingClient.exe "888888005" "P@ssword" "https://dptng.njcourts.gov:2045/civilFilingWS_t" "C:\Files\File2.xml" 
C:\Users\CivilFilingClient> CivilFilingClient.exe "888888005" "P@ssword" "https://dptng.njcourts.gov:2045/civilFilingWS_t" "C:\Files\File3.xml" 
C:\Users\CivilFilingClient> CivilFilingClient.exe "888888005" "P@ssword" "https://dptng.njcourts.gov:2045/civilFilingWS_t" "C:\Files\File4.xml" 
C:\Users\CivilFilingClient> CivilFilingClient.exe "888888005" "P@ssword" "https://dptng.njcourts.gov:2045/civilFilingWS_t" "C:\Files\File5.xml" 

```

If you name the file using the eCourtSuits.bat, you can setup this to be ran daily in the Windows Task Scheduler once you are ok with the processing.

Installation

------------
Run the CivilFilingClient.msi or Setup.exe

Dependencies

------------

* New Jersey Courts Web Service Endpoints
* NLog

Application Setting (CivilFilingClient.exe.config)

------------
The following application settings are required.

```xml

  <appSettings>
    <add key="productionEndPoint" value="https://dpprod.njcourts.gov:2045/civilFilingWS_p"/>
    <add key="productionUsername" value="333333333"/>
    <add key="productionPwd" value="53c3t"/>
    <add key="testEndPoint" value="https://dptng.njcourts.gov:2045/civilFilingWS_t"/>
    <add key="testUsername" value="888888005"/>
    <add key="testPwd" value="123987GJGJJ$"/>
    <!-- mode is 'Test' -> testEndPoint and mode is 'Production' -> productionEndpoint-->
    <add key="mode" value="Test"/>
  </appSettings>

```

Configuration Bindings (CivilFilingClient.exe.config)

------------
Bindings setup the transport security the NJ soap web service endpoint.  
The client node configures the channel for the NJ soap web service endpoint.

The soap endpoint requires a specific soap header with credentials.  All
communication is over https so the credentials are encrypted.

```xml
    <bindings>
      <basicHttpBinding>
        <binding name="CivilFilingWSPortBinding" >
          <security mode="Transport">
            <transport clientCredentialType="None" />
          </security>
        </binding>
        <binding name="CivilFilingWSService_CivilFilingWSPort" messageEncoding="Mtom">
          <security mode="Transport">
            <transport clientCredentialType="None" />
          </security>
        </binding>
      </basicHttpBinding>
    </bindings>
    <client>
      <endpoint address="https://dptng.njcourts.gov:2045/civilFilingWS_t"
          binding="basicHttpBinding"
          bindingConfiguration="CivilFilingWSService_CivilFilingWSPort"
          contract="CivilFilingServiceReference.CivilFilingWS"
          name="CivilFilingWSPort">
        <headers>
          <wsse:Security xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
            <wsse:UsernameToken Id="unt_20">
              <wsse:Username>888888005</wsse:Username>
              <wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">P@ssword</wsse:Password>
            </wsse:UsernameToken>
          </wsse:Security>
        </headers>
      </endpoint>
    </client>

```

## Security Header Example

```xml

<wsse:Security xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
  <wsse:UsernameToken Id="unt_20">
    <wsse:Username>123123123</wsse:Username>
    <wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">PASSWORD</wsse:Password>
  </wsse:UsernameToken>
</wsse:Security>

```

### Test Files

* TEST_CMP2_TESTDAN.XML
* TEST_CMP_TESTDAN.PDF
* TEST_CMP_TESTDAN_addBranchId.XML
* TestCorp2Corp.XML
* TestCorp2Corp.pdf
* TestCorp2CorpBAD.XML
* TestCorp2CorpBAD.pdf
* TestCorp2CorpCDATA.XML
* TestCorp2CorpCDATA.pdf
* TestCorp2Corp_MissingBranchID.XML
* TestCorp2Corp_MissingBranchID.pdf
* TestCorp2Individ.XML
* TestCorp2Individ.pdf
* TestIndivid2Corp.XML
* TestIndivid2Corp.pdf
* TestIndivid2Individ.XML
* TestIndivid2Individ.pdf

## Sample Soap Message

This is a sample message from the New Jeresy Court web service specification

```xml

<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" 
                  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
                  xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                  xmlns:tns2="http://webservice.civilfiling.ecourts.ito.aoc.nj/">
  <soapenv:Body>
    <tns2:submitCivilFiling>
      <arg0>
        <bulkFilingPacket>
          <attachmentList>
            <contentType>application/pdf</contentType>
            <docType>pdf</docType>
            <documentCode>CMPL</documentCode>
            <documentDescription>Complaint</documentDescription>
            <documentName>Test</documentName>
            <extention>.pdf</extention>
          </attachmentList>
          <attorneyFirmId>F00004861</attorneyFirmId>
          <attorneyId>012152010</attorneyId>
          <civilCase>
            <caseAction>028</caseAction>
            <courtSection>SCP</courtSection>
            <defendantCaption>test def caption</defendantCaption>
            <demandAmount>1000</demandAmount>
            <docketDetailsForOtherCourt>str</docketDetailsForOtherCourt>
            <juryDemand>N</juryDemand>
            <lawFirmCaseId>100</lawFirmCaseId>
            <otherCourtActions>str</otherCourtActions>
            <plaintiffCaption>test plaintiff caption</plaintiffCaption>
            <serviceMethod>03</serviceMethod>
            <venue>ATL</venue>
            <venueOfIncident>ATL</venueOfIncident>
          </civilCase>
          <defendantList>
            <adaAccommodationInd>N</adaAccommodationInd>
            <corporationName>XYZ Corp</corporationName>
            <corporationType>CO</corporationType>
            <interpreterInd>N</interpreterInd>
            <partyAffiliation>ADM</partyAffiliation>
            <partyDescription>BUS</partyDescription>
          </defendantList>
          <fee>
            <accountNumber>34550</accountNumber>
            <attorneyClientRefNumber>12345</attorneyClientRefNumber>
            <attorneyFee>100</attorneyFee>
            <feeExempt>N</feeExempt>
            <paymentType>CGr</paymentType>
          </fee>
          <plaintiffList>
            <adaAccommodationInd>N</adaAccommodationInd>
            <address>
              <addressLine1>market st</addressLine1>
              <city>Trenton</city>
              <stateCode>NJ</stateCode>
              <zipCode>12345</zipCode>
            </address>
            <corporationName>ABC Corp</corporationName>
            <corporationType>CO</corporationType>
            <interpreterInd>N</interpreterInd>
            <partyAffiliation>ADM</partyAffiliation>
            <partyDescription>BUS</partyDescription>
          </plaintiffList>
        </bulkFilingPacket>
      </arg0>
    </tns2:submitCivilFiling>
  </soapenv:Body>
</soapenv:Envelope>

```
Sample Message Generated by this Application

```xml

<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
  <s:Header>
    <wsse:Security xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
      <wsse:UsernameToken Id="unt_20">
        <wsse:Username>888888005</wsse:Username>
        <wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">P@ssword</wsse:Password>
      </wsse:UsernameToken>
    </wsse:Security>
  </s:Header>
  <s:Body xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    <submitCivilFiling xmlns="http://webservice.civilfiling.ecourts.ito.aoc.nj/">
      <arg0 xmlns="">
        <bulkFilingPacket>
          <attachmentList>
            <bytes>
              <xop:Include href="cid:http://tempuri.org/1/636103044045015185" xmlns:xop="http://www.w3.org/2004/08/xop/include"/>
            </bytes>
            <contentType>application/pdf</contentType>
            <docType>pdf</docType>
            <documentCode>CMPL</documentCode>
            <documentDescription>Complaint</documentDescription>
            <documentName>Test</documentName>
            <extention>pdf</extention>
          </attachmentList>
          <attorneyId>000000000</attorneyId>
          <civilCase>
            <caseAction>028</caseAction>
            <courtSection>SCP</courtSection>
            <defendantCaption>test Def caption</defendantCaption>
            <juryDemand>N</juryDemand>
            <lawFirmCaseId>100</lawFirmCaseId>
            <otherCourtActions>N</otherCourtActions>
            <plaintiffCaption>test plantiff caption</plaintiffCaption>
            <serviceMethod>03</serviceMethod>
            <venue>ATL</venue>
            <venueOfIncident>ATL</venueOfIncident>
          </civilCase>
          <defendantList>
            <accommodationType>a</accommodationType>
            <adaAccommodationInd>N</adaAccommodationInd>
            <address>
              <addressLine1>25, market street</addressLine1>
              <addressLine2>Trenton</addressLine2>
              <stateCode>NJ</stateCode>
              <zipCode>12345</zipCode>
            </address>
            <corporationName>ABC Corp</corporationName>
            <corporationType>CO</corporationType>
            <interpreterInd>N</interpreterInd>
            <partyAffiliation>ADM</partyAffiliation>
            <partyDescription>BUS</partyDescription>
          </defendantList>
          <documentRedactionInd>Y</documentRedactionInd>
          <fee>
            <accountNumber>12345</accountNumber>
            <attorneyClientRefNumber>12345</attorneyClientRefNumber>
            <feeExempt>N</feeExempt>
            <paymentType>CG</paymentType>
          </fee>
          <plaintiffList>
            <accommodationType>a</accommodationType>
            <adaAccommodationInd>N</adaAccommodationInd>
            <address>
              <addressLine1>25, market street</addressLine1>
              <addressLine2>Trenton</addressLine2>
              <stateCode>NJ</stateCode>
              <zipCode>12345</zipCode>
            </address>
            <corporationName>ABC Corp</corporationName>
            <corporationType>CO</corporationType>
            <interpreterInd>N</interpreterInd>
            <language>SPA</language>
            <partyAffiliation>ADM</partyAffiliation>
            <partyDescription>BUS</partyDescription>
          </plaintiffList>
        </bulkFilingPacket>
      </arg0>
    </submitCivilFiling>
  </s:Body>
</s:Envelope>

```

Successful Message from eCourts

------------

Note: We should mention to them the spelling error.

```txt

ECCV500 Description: Filing submitted scuccessfully
Efiling Sequence Number :SCP20161007536
Docket Number :ATL-DC-005336-16 

```

Error Messages (samples from eCourts)

------------
More messages can be found in the specifications document.

See the specification for the error codes which the web service will return.

### Security (Username or Password failed)

```txt

Rejected by policy. (from client) 

```

When you receive this message either or both the username and password are invalid.

#### http timeout

```txt

The HTTP request to 'https://dptng.njcourts.gov:2045/civilFilingWS_t' has exceeded the allotted timeout of 00:00:59.9680000. 
The time allotted to this operation may have been a portion of a longer timeout.

```

When you see this error communication with the webservice was disconnected before
the submission could be created or the web service (eCourts) could respond back to the client (CivilFilingClient).

TODO: We can set the timeout to be longer if needed in the future.

### Missing Branch Id

```txt

ECCV200 Description: Branch Id cannot be null or empty

```

### Invalid Characters

```txt

ECCV110 Description: Party name can only contain A-Z, a-z, 0-9, space, period, dash, $, ?, !, (, ), #, %, comma, slash, single quote, &

```

### eCourt returns only Sequence Number

The eCourt web service was down and not processing the submissions.  The request will be accepted and return a sequence number
but no Docket Number.  You will have to resend this submission once the web service is back up.

```txt

eFiling seq number:SCP20161007426

```

### Additional Messages

```txt

Code: ECCV200 Description: Please provide a valid Firm/ branch ID
Code: ECCV100 Description: Document Redaction Indicator should be Y 
Code: ECCV110 Description: Attorney's Client Reference number should be a numeric value [0-9]  


```

### File Name Valid characters

A-Z, a-z, 0-9, .

Note, This application might need to validate the filename, we can test and see what
happens when I send a pdf file with characters which are not valid.

### Text Valid characters

A-Z, a-z, 0-9, space, period, dash, $, ?, !, (, ), #, %, comma, slash, single quote, &

Inspecting the soap message

------------
You can inspect the soap message by creating an endpoint and sending the request to the endpoint.
You will need to comment out the security in the App.config.

```xml

<security mode="Transport">
  <transport clientCredentialType="None" />
</security>

```

This will allow you to see the message in clear text.

You can also install and enable Wireshark on windows to decrypt the https traffic you generate.

Logging

------------

Logs are written for each run and are located in the logs folder inside the applications folder.
The logs are setup to maintain up to 365 days of logs. 

Web Service Creation

------------

GUI

In Visual Studio Right Click on CivilFilingClient>ServiceReferences

- Add Service Reference
- Address (Your path to the wsdl in a folder with the xsd file) : c:\source\repos\civilfiling\test\CivilFilingWSService.wsdl
- Add your namespace:  CivilFilingClient[Version]

References

------------

https://dptng.njcourts.gov:2045/civilFilingWS_t

http://stackoverflow.com/questions/848841/c-sharp-xslt-transform-adding-xa-and-xd-to-the-output

http://stackoverflow.com/questions/14327960/with-c-wcf-soap-consumer-that-uses-wsse-plain-text-authentication/14334760

https://ecourtstraining.judiciary.state.nj.us/webe19/CIVILCaseJacketWeb/pages/civilCaseSearch.faces

http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd

### Registration

Test: https://ecourtstraining.judiciary.state.nj.us/webe19/ecourtsweb/pages/home/home.faces 
username: 888888005
password: P@ssword

Your email address has been successfully added for eCourts - electronic filing, service and notification in the New Jersey Trial Courts. The email addresses provided will be used for electronic notifications on all eCourts filings.

Attorney Name :TEST5 CIVIL BULK FILING5
Attorney Bar ID :888888005
Firm Name :CIVIL BULK FILING TEST3

Please use the following for attorney, firm and branch id. Attached is the updated documentation. Branch id is a required field and it can be in the attributes list under BulkFilingpacket

### Testing Account

Firm Id - F88888003
Attorney ID - 888888005
Branch Id - 0001
Account number for fee processing - 143055

If you do not have one, please use the following and this is for testing purposes only

 
Updated Values
Attorney id  - 888888029
Firm Id - F88888015
Branch id – 0001
JACS account - 143089

Branch Id and Account Number are required for processing. See the eCourts specification for more information about the required elements.

Web Service Credentials
username: 888888005
password: P@ssword


Example for using with go soap server to inspect the soap message

http://localhost:8081/soapserver

---------SOAP SERVER TEST CONFIG

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
  </configSections>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
  </startup>
  <appSettings>
    <add key="productionEndPoint" value="http://localhost:8081/soapserver"/>
    <add key="productionUsername" value="F00012033"/>
    <add key="productionPwd" value="x8xNbC$c"/>
    <add key="testEndPoint" value="http://localhost:8081/soapserver"/>
    <add key="testUsername" value="888888029"/>
    <add key="testPwd" value="P@ssword"/>
    <!-- mode is 'Test' -> testEndPoint and mode is 'Production' -> productionEndpoint-->
    <add key="mode" value="Test"/>
  </appSettings>
  <system.serviceModel>
    <bindings>
      <basicHttpBinding>
        <binding name="CivilFilingWSPortBinding" sendTimeout="00:03:00">
          <security mode="None">
            <transport clientCredentialType="None" />
          </security>
        </binding>
        <binding name="CivilFilingWSService_CivilFilingWSPort" sendTimeout="00:03:00"
          messageEncoding="Mtom">
          <security mode="None">
            <transport clientCredentialType="None" />
          </security>
        </binding>
      </basicHttpBinding>
    </bindings>
    <client>
      <endpoint address="https://dptng.njcourts.gov:2045/civilFilingWS_t"
          binding="basicHttpBinding"
          bindingConfiguration="CivilFilingWSService_CivilFilingWSPort"
          contract="CivilFilingServiceReference.CivilFilingWS"
          name="CivilFilingWSPort">
        <headers>
          <wsse:Security xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
            <wsse:UsernameToken Id="unt_20">
              <wsse:Username>888888005</wsse:Username>
              <wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">P@ssword</wsse:Password>
            </wsse:UsernameToken>
          </wsse:Security>
        </headers>
      </endpoint>
    </client>
  </system.serviceModel>
</configuration>



-------------------------GOOD CONFIG
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
  </configSections>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
  </startup>
  <appSettings>
    <add key="productionEndPoint" value="https://dpprod.njcourts.gov:2045/civilFilingWS_p"/>
    <add key="productionUsername" value="F00012033"/>
    <add key="productionPwd" value="x8xNbC$c"/>
    <add key="testEndPoint" value="https://dptng.njcourts.gov:2045/civilFilingWS_t"/>
    <add key="testUsername" value="888888029"/>
    <add key="testPwd" value="P@ssword"/>
    <!-- mode is 'Test' -> testEndPoint and mode is 'Production' -> productionEndpoint-->
    <add key="mode" value="Test"/>
  </appSettings>
  <system.serviceModel>
    <bindings>
      <basicHttpBinding>
        <binding name="CivilFilingWSPortBinding" sendTimeout="00:03:00">
          <security mode="Transport">
            <transport clientCredentialType="None" />
          </security>
        </binding>
        <binding name="CivilFilingWSService_CivilFilingWSPort" sendTimeout="00:03:00"
          messageEncoding="Mtom">
          <security mode="Transport">
            <transport clientCredentialType="None" />
          </security>
        </binding>
      </basicHttpBinding>
    </bindings>
    <client>
      <endpoint address="https://dptng.njcourts.gov:2045/civilFilingWS_t"
          binding="basicHttpBinding"
          bindingConfiguration="CivilFilingWSService_CivilFilingWSPort"
          contract="CivilFilingServiceReference.CivilFilingWS"
          name="CivilFilingWSPort">
        <headers>
          <wsse:Security xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
            <wsse:UsernameToken Id="unt_20">
              <wsse:Username>888888005</wsse:Username>
              <wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">P@ssword</wsse:Password>
            </wsse:UsernameToken>
          </wsse:Security>
        </headers>
      </endpoint>
    </client>
  </system.serviceModel>
</configuration>
