# Civil Filing Client

Requirements
------------
Law firm in NJ uses some old system written in some BASIC variant to interact with NJ Legal system.
NJ Courts moving to E-Filing over SOAP.

Write a simple win form app that loads XML files and corresponding attachments 
from a directory and submits them over the SOAP service.  

Save output of SOAP service into a text file in the same directory.

Use Cases
------------

## 1
User will select one xml and one pdf file to be loaded and sent to New Jersey Courts
The one xml and one pdf file will be associated with the case.

The response from the web service will be written to the same directory the xml and pdf
originate from.

# 2 
User selects one file from on directory, closes diaglog window, and then opens it again
to select the other file from a differnt directory.




Installation
------------

Dependencies
------------

Configuration
------------
New Jeresy Courts Web Service
username
password


Sample Message
--------------


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


Web Service Takes
filingRequest


Security 
------------

Logging
------------


https://dptng.njcourts.gov:2045/civilFilingWS_t

<endpoint address="https://172.16.248.34:2045/civilFilingWS_t"

http://localhost:8081/soapserver

http://stackoverflow.com/questions/848841/c-sharp-xslt-transform-adding-xa-and-xd-to-the-output

http://stackoverflow.com/questions/14327960/with-c-wcf-soap-consumer-that-uses-wsse-plain-text-authentication/14334760

https://ecourtstraining.judiciary.state.nj.us/webe19/CIVILCaseJacketWeb/pages/civilCaseSearch.faces

http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd


C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\CivilFilingClient


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

