# Civil Filing Client

Requirements
------------
Law firm in NJ uses some old system written in some BASIC variant to interact with NJ Legal system.
NJ Courts moving to E-Filing over SOAP.

Write a simple windows form app that loads XML files and corresponding pdf attachments 
from a directory and submits them over the SOAP service.  

Save output of SOAP service into a text file in the same directory.

Use Cases
------------

## 1
User will select one xml and one pdf file to be loaded and sent to New Jersey Courts
The one xml and one pdf file will be associated with the case.

The response from the web service will be written to the same directory the xml and pdf
originate from.

## Assumptions
 * xml and pdf file will be in same directory
 * we only expect to have one xml and pdf file per submission



Installation
------------
TODO: Create msi installer

Dependencies
------------
* New Jeresy Courts Web Service
* NLog


Configuration
------------
App.config (Development) or CivilFilingClient.exe.config (Production).

```xml

        <client>
          <endpoint address="https://dptng.njcourts.gov:2045/civilFilingWS_t"
              binding="basicHttpBinding"
              bindingConfiguration="CivilFilingWSService_CivilFilingWSPort"
              contract="CivilFilingServiceReference.CivilFilingWS"
              name="CivilFilingWSPort">
            <headers>
              <wsse:Security xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"><wsse:UsernameToken Id="unt_20"><wsse:Username>888888005</wsse:Username><wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">P@ssword</wsse:Password></wsse:UsernameToken></wsse:Security>
            </headers>
          </endpoint>
        </client>

```
Endpoint Address is the url of the New Jersey Courts web service.

The security is in the <headers/> element.  The header element needs to be one line for now, since
adding in CRLF adds '>&#xD' at each new line character.

Setting the web service username and password is done in the security header.

```xml

<wsse:Security xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
	<wsse:UsernameToken Id="unt_20">
		<wsse:Username>888888005</wsse:Username>
		<wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">P@ssword</wsse:Password>
	</wsse:UsernameToken>
</wsse:Security>

```

Test Files
--------------
There is a folder named TestFiles which contains 3 files.
* MessageSample1.XML
* some.pdf
* Responses.txt

MessageSample1.xml should be what the law firm will try and upload.  This is my guess for now.
The pdf is just a sample pdf.
Responses.txt contains the output of the processing as required by the scope or this work.

Currently I keep getting this error message:

```txt
Code: ECCV200 Description: Branch Id cannot be null or empty
```

And I'm not seeing the branch Id in the xml objects to send.  :-(


Sample Soap Message
--------------
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

Error Message from New Jersy Courts
------------
See the specification for the error codes which the web service will return.

### File Name Valid characters
A-Z, a-z, 0-9, .

Note, This application might need to validate the filename, we can test and see what
happens when I send a pdf file with characters which are not valid.

### Text Valid characters
A-Z, a-z, 0-9, space, period, dash, $, ?, !, (, ), #, %, comma, slash, single quote, &

Inspecting the soap message 
------------
One can inspect the soap message by creating an endpoint and sending the request to the endpoint.
You will need to comment out the security in the App.config.

```xml

<security mode="Transport">
	<transport clientCredentialType="None" />
</security>

```
This will allow you to see the message in clear text.

Logging
------------
Logs are written for each run and are located in the logs folder inside the applications folder.
The logs are setup to maintain up to 365 days of logs. 

References
------------
https://dptng.njcourts.gov:2045/civilFilingWS_t

http://stackoverflow.com/questions/848841/c-sharp-xslt-transform-adding-xa-and-xd-to-the-output

http://stackoverflow.com/questions/14327960/with-c-wcf-soap-consumer-that-uses-wsse-plain-text-authentication/14334760

https://ecourtstraining.judiciary.state.nj.us/webe19/CIVILCaseJacketWeb/pages/civilCaseSearch.faces

http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd

Testing
-------------
http://localhost:8081/soapserver

Notes
-------------

Step 1 Registration
Test: https://ecourtstraining.judiciary.state.nj.us/webe19/ecourtsweb/pages/home/home.faces 

username: 888888005
password: P@ssword

Your email address has been successfully added for eCourts - electronic filing, service and notification in the New Jersey Trial Courts. The email addresses provided will be used for electronic notifications on all eCourts filings.

Attorney Name :	TEST5 CIVIL BULK FILING5
Attorney Bar ID :	888888005
Firm Name :	CIVIL BULK FILING TEST3

Code: ECCV200 Description: Branch Id cannot be null or empty
There is no branch in the schema?

Firm id = 9735384700
Atty id = 288551973


Please use the following for attorney, firm and branch id. Attached is the updated documentation. Branch id is a required field and it can be in the attributes list under BulkFilingpacket
 
Testing Account		
Firm Id - F88888003
Attorney ID – 888888005/ 888888006     
Branch Id – 0001
Account number for fee processing - 143055       