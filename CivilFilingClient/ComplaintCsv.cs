using System;
using System.Text.RegularExpressions;

namespace CivilFilingClient
{
    class ComplaintCsv
    {
        public string AttorneyId;
        public string AttorneyFirmId;
        public string BranchId;

        public string CourtSection;
        public string Venue;
        public string CertifcationText;
        public string Action;
        public string DemandAmount;
        public string JuryDemand;
        public string ServiceMethod;
        public string LawFirmCaseId;
        public string CountyOfIncident;
        public string PlantiffCaption;
        public string DefendantCaption;

        public string PartyDescription_Plaintiff;
        public string PartyAffiliation_Plaintiff;
        public string CorporationType_Plaintiff;
        public string CorpName_Plaintiff;
        public string Phone_Plaintiff;
        public string InterpreterNeeded_Plaintiff;
        public string Language_Plaintiff;
        public string AccomodationNeeded_Plaintiff;
        public string AccomodationRequirement_Plaintiff;
        public string AdditionalAccomodationDetails_Plaintiff;

        public string FirstName_Plaintiff;
        public string MiddleInitial_Plaintiff;
        public string LastName_Plaintiff;

        public string AddressLine1_Plaintiff;
        public string AddressLine2_Plaintiff;
        public string City_Plaintiff;
        public string State_Plaintiff;
        public string ZipCode_Plaintiff;
        public string ZipCodeExt_Plaintiff;

        public string AlternateType_Plaintiff;
        public string AlternateName_Plaintiff;

        public string PartyDescription_Defendant;
        public string PartyAffiliation_Defendant;
        public string CorporationType_Defendant;
        public string Name_Defendant;
        public string Phone_Defendant;

        public string FirstName_Defendant;
        public string MiddleInitial_Defendant;
        public string LastName_Defendant;

        public string AddressLine1_Defendant;
        public string AddressLine2_Defendant;
        public string City_Defendant;
        public string State_Defendant;
        public string ZipCode_Defendant;
        public string ZipCodeExt_Defendant;
        public string AlternateType_Defendant;
        public string AlternateName_Defendant;
        public string PDF_FileLocation;
        public string AttorneyFee;
        public string PaymentMethod;
        public string AccountNumber;
        public string AttorneyClientRefNumber;
        public string FeeExempt;
        public string ReasonForFilingFeeExemption;
    }
}
