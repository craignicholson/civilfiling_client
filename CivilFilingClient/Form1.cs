﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CivilFilingClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            // We will only attach the pdf... but we can have many pdf's
            // Use Case - we will file one case at a time.
            // Use Case, One xml file and multiple pdfs? I think this is how it should be.
            // xml file can have many plantiff and defendants?
            // 
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            CivilFilingServiceReference.@case caseData = new CivilFilingServiceReference.@case();
            caseData.caseAction="028";
            caseData.courtSection = "SCP";
            caseData.defendantCaption = "test Def caption";
            caseData.demandAmount = new decimal(1000);
            caseData.plaintiffCaption ="test plantiff caption";
            caseData.juryDemand = "N";
            caseData.lawFirmCaseId = "100";
            caseData.otherCourtActions = "N";
            caseData.serviceMethod = "03";
            caseData.venue = "ATL";
            caseData.venueOfIncident = "ATL";

            CivilFilingServiceReference.attachment att = new CivilFilingServiceReference.attachment();
            string filePath = @"C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\CivilFilingClient\CivilFilingClient\some.pdf";
            //TODO: how large are the files?
            byte[] bytes = File.ReadAllBytes(filePath);

            att.bytes = bytes;
            att.contentType = "application/pdf";
            att.docType = "pdf";
            att.documentCode = "CMPL";
            att.documentDescription = "Complaint";
            att.documentName = "Test";
            att.extention = "pdf";

            CivilFilingServiceReference.fee fee = new CivilFilingServiceReference.fee();
            fee.accountNumber = "12345";
            fee.attorneyClientRefNumber = "12345";
            fee.attorneyFee = new decimal(1000);
            fee.feeExempt = "N";
            fee.paymentType = "CG";

            CivilFilingServiceReference.address pAddress = new CivilFilingServiceReference.address();
            pAddress.addressLine1 = "25, market street";
            pAddress.addressLine2 = "Trenton";
            pAddress.stateCode = "NJ";
            pAddress.zipCode = "12345";

            CivilFilingServiceReference.party plantiff = new CivilFilingServiceReference.party();
            plantiff.adaAccommodationInd = "N";
            plantiff.address = pAddress;
            plantiff.corporationName = "ABC Corp";
            plantiff.corporationType = "CO";
            plantiff.interpreterInd = "N";
            plantiff.partyAffiliation = "ADM";
            plantiff.partyDescription = "BUS";
            plantiff.interpreterInd = "N";
            plantiff.language = "SPA";
            plantiff.accommodationType = "a";

            CivilFilingServiceReference.party defendant = new CivilFilingServiceReference.party();
            defendant.adaAccommodationInd = "N";
            defendant.address = pAddress;
            defendant.corporationName = "ABC Corp";
            defendant.corporationType = "CO";
            defendant.interpreterInd = "N";
            defendant.partyAffiliation = "ADM";
            defendant.partyDescription = "BUS";
            defendant.accommodationType = "a";

            CivilFilingServiceReference.bulkFilingPacket packet = new CivilFilingServiceReference.bulkFilingPacket();
            //takes and array of attachment[] we just have attachment
            CivilFilingServiceReference.attachment[] attachments = new CivilFilingServiceReference.attachment[1];
            attachments[0] = att;
            packet.attachmentList = attachments;
            packet.attorneyId = "000000000";
            packet.attorneyId = "000000000";

            packet.civilCase = caseData;
            int numberOfDefendants = 1;
            CivilFilingServiceReference.party[] defendants = new CivilFilingServiceReference.party[numberOfDefendants];
            defendants[0] = defendant;
            packet.defendantList = defendants;

            packet.documentRedactionInd = "Y";
            packet.fee = fee;

            int numberOfPlantiffs = 1;
            CivilFilingServiceReference.party[] plantiffs = new CivilFilingServiceReference.party[numberOfPlantiffs];
            plantiffs[0] = plantiff;
            packet.plaintiffList = plantiffs;
            

            CivilFilingServiceReference.civilFilingRequest filingRequest = new CivilFilingServiceReference.civilFilingRequest();
            filingRequest.bulkFilingPacket= packet;
            
            // Create the proxy
            // Set the correct endpoint... this is working differnt than expected... old soap stuff...
            // Add the security in the header
            // Send the Request
            // Wait for the reponse
            // parse out the responses
            CivilFilingServiceReference.CivilFilingWSClient proxy = new CivilFilingServiceReference.CivilFilingWSClient();
            // Add the special security header
            //<s:Security soapenv:mustUnderstand="1" xmlns:s="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" xmlns:u="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
            //  <s:UsernameToken u:Id="unt_20">
            //      <s:Username>XXXXXXXXX</s:Username>
            //      <s:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">XXXXXXXXX</s:Password>
            //  </s:UsernameToken>
            //</s:Security>
            

            Debug.Print(proxy.Endpoint.ToString());
            //TODO: Put error checking around the web service call
            try
            {
                CivilFilingServiceReference.civilFilingResponse filingReponse = proxy.submitCivilFiling(filingRequest);

                foreach (var msg in filingReponse.messages)
                {
                    Debug.Print("Code: " + msg.code + " Description: " + msg.description);
                }
                if (filingReponse.efilingNumber != null)
                {
                    Debug.Print("eFiling seq number:");
                    Debug.Print(filingReponse.efilingNumber.efilingCourtDiv.ToString());
                    Debug.Print(filingReponse.efilingNumber.efilingCourtYr.ToString());
                    Debug.Print(filingReponse.efilingNumber.efilingSeqNo.ToString());
                }
                if (filingReponse.docketNumber != null)
                {
                    Debug.Print("Docket number:");
                    Debug.Print(filingReponse.docketNumber.docketVenue.ToString());
                    Debug.Print(filingReponse.docketNumber.docketTypeCode.ToString());
                    Debug.Print(filingReponse.docketNumber.docketCourtYear.ToString());
                    Debug.Print(filingReponse.docketNumber.docketSeqNum.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }
    }
}