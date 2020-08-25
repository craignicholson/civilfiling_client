using System;
using System.Text.RegularExpressions;

namespace CivilFilingClient
{
    class ComplaintCsv
    {
        int AttorneyId;
        int AttorneyFirmId;
        string BranchId;

        string CourtSection;
        string Venue;
        string CertifcationText;
        string Action;
        string DemandAmount;
        string JuryDemand;
        string ServiceMethod;
        string LawFirmCaseId;
        string CountyOfIncident;
        string PlantiffCaption;
        string DefendantCaption;

        string PartyDescription_Plaintiff;
        string PartyAffiliation_Plaintiff;
        string CorporationType_Plaintiff;
        string CorpName_Plaintiff;
        string Phone_Plaintiff;
        string InterpreterNeeded_Plaintiff;
        string Language_Plaintiff;
        string AccomodationNeeded_Plaintiff;
        string AccomodationRequirement_Plaintiff;
        string AdditionalAccomodationDetails_Plaintiff;

        string FirstName_Plaintiff;
        string MiddleInitial_Plaintiff;
        string LastName_Plaintiff;

        string AddressLine1_Plaintiff;
        string AddressLine2_Plaintiff;
        string City_Plaintiff;
        string State_Plaintiff;
        string ZipCode_Plaintiff;
        string ZipCodeExt_Plaintiff;
 
        string AlternateType_Plaintiff;
        string AlternateName_Plaintiff;

        string PartyDescription_Defendant;
        string PartyAffiliation_Defendant;
        string CorporationType_Defendant;
        string Name_Defendant;
        string Phone_Defendant;

        string FirstName_Defendant;
        string MiddleInitial_Defendant;
        string LastName_Defendant;

        string AddressLine1_Defendant;
        string AddressLine2_Defendant;
        string City_Defendant;
        string State_Defendant;
        string ZipCode_Defendant;
        string ZipCodeExt_Defendant;
        string AlternateType_Defendant;
        string AlternateName_Defendant;
        string PDF_FileLocation;
        string AttorneyFee;
        string PaymentMethod;
        string AccountNumber;
        string AttorneyClientRefNumber;
        string FeeExempt;
        string ReasonForFilingFeeExemption;

        /// <summary>
        /// TODO: ADD IN CHECKS AND PASS IN LOGGING
        /// </summary>
        /// <param name="csvLine"></param>
        /// <returns></returns>
        public static ComplaintCsv FromCsv(string csvLine)
        {

            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            string[] values = CSVParser.Split(csvLine);
            ComplaintCsv data = new ComplaintCsv();

            // ComplaintCsv.Date = Convert.ToDateTime(values[0]);
            data.AttorneyId = Convert.ToInt16(values[0]);
            data.AttorneyFirmId = Convert.ToInt16(values[1]);
            data.BranchId = values[2];
            
            data.CourtSection = values[3];
            data.Venue = values[4];
            data.CertifcationText = values[5];
            data.Action = values[6];
            data.DemandAmount = values[7];
            data.JuryDemand = values[8];
            data.ServiceMethod = values[9];
            data.LawFirmCaseId = values[10];
            data.CountyOfIncident = values[11];
            data.PlantiffCaption = values[12];
            data.DefendantCaption = values[13];

            data.PartyDescription_Plaintiff = values[14];
            data.PartyAffiliation_Plaintiff = values[15];
            data.CorporationType_Plaintiff = values[16];
            data.CorpName_Plaintiff = values[17];
            data.Phone_Plaintiff = values[18];
            data.InterpreterNeeded_Plaintiff = values[19];
            data.Language_Plaintiff = values[20];
            data.AccomodationNeeded_Plaintiff = values[21];
            data.AccomodationRequirement_Plaintiff = values[22];
            data.AccomodationRequirement_Plaintiff = values[23];
            data.AdditionalAccomodationDetails_Plaintiff = values[24];

            data.FirstName_Plaintiff = values[25];
            data.MiddleInitial_Plaintiff = values[26];
            data.LastName_Plaintiff = values[27];

            data.AddressLine1_Plaintiff = values[28];
            data.AddressLine2_Plaintiff = values[29];
            data.City_Plaintiff = values[30];
            data.State_Plaintiff = values[31];
            data.ZipCode_Plaintiff = values[32];
            data.ZipCodeExt_Plaintiff = values[33];

            data.AlternateType_Plaintiff = values[34];
            data.AlternateName_Plaintiff = values[35];

            data.PartyDescription_Defendant = values[36];
            data.PartyAffiliation_Defendant = values[37];
            data.CorporationType_Defendant = values[38];
            data.Name_Defendant = values[39];
            data.Phone_Defendant = values[40];

            data.FirstName_Defendant = values[41];
            data.MiddleInitial_Defendant = values[42];
            data.LastName_Defendant = values[43];

            data.AddressLine1_Defendant = values[44];
            data.AddressLine2_Defendant = values[45];
            data.City_Defendant = values[46];
            data.State_Defendant = values[47];
            data.ZipCode_Defendant = values[48];
            data.ZipCodeExt_Defendant = values[49];

            data.AlternateType_Defendant = values[50];
            data.AlternateName_Defendant = values[51];

            data.PDF_FileLocation = values[52];

            data.AttorneyFee = values[53];
            data.PaymentMethod = values[54];
            data.AccountNumber = values[55];
            data.AttorneyClientRefNumber = values[56];
            data.FeeExempt = values[57];
            data.ReasonForFilingFeeExemption = values[58];

            return data;
        }
    }
}
