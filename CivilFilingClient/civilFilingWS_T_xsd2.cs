﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/", IsNullable=false)]
public partial class ECourtsCivilServiceException {
    
    private string messageField;
    
    private string messageCodeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string message {
        get {
            return this.messageField;
        }
        set {
            this.messageField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string messageCode {
        get {
            return this.messageCodeField;
        }
        set {
            this.messageCodeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class message {
    
    private string codeField;
    
    private string descriptionField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string code {
        get {
            return this.codeField;
        }
        set {
            this.codeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class docketNumber {
    
    private short docketCenturyYearField;
    
    private short docketCourtYearField;
    
    private long docketSeqNumField;
    
    private string docketTypeCodeField;
    
    private string docketVenueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public short docketCenturyYear {
        get {
            return this.docketCenturyYearField;
        }
        set {
            this.docketCenturyYearField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public short docketCourtYear {
        get {
            return this.docketCourtYearField;
        }
        set {
            this.docketCourtYearField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public long docketSeqNum {
        get {
            return this.docketSeqNumField;
        }
        set {
            this.docketSeqNumField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string docketTypeCode {
        get {
            return this.docketTypeCodeField;
        }
        set {
            this.docketTypeCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string docketVenue {
        get {
            return this.docketVenueField;
        }
        set {
            this.docketVenueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class civilFilingResponse {
    
    private docketNumber docketNumberField;
    
    private efilingNumber efilingNumberField;
    
    private message[] messagesField;
    
    private bool queueFilingProcessedField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public docketNumber docketNumber {
        get {
            return this.docketNumberField;
        }
        set {
            this.docketNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public efilingNumber efilingNumber {
        get {
            return this.efilingNumberField;
        }
        set {
            this.efilingNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("messages", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public message[] messages {
        get {
            return this.messagesField;
        }
        set {
            this.messagesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool queueFilingProcessed {
        get {
            return this.queueFilingProcessedField;
        }
        set {
            this.queueFilingProcessedField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class efilingNumber {
    
    private string efilingCourtDivField;
    
    private string efilingCourtYrField;
    
    private long efilingSeqNoField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string efilingCourtDiv {
        get {
            return this.efilingCourtDivField;
        }
        set {
            this.efilingCourtDivField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string efilingCourtYr {
        get {
            return this.efilingCourtYrField;
        }
        set {
            this.efilingCourtYrField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public long efilingSeqNo {
        get {
            return this.efilingSeqNoField;
        }
        set {
            this.efilingSeqNoField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class fee {
    
    private string accountNumberField;
    
    private string attorneyClientRefNumberField;
    
    private decimal attorneyFeeField;
    
    private bool attorneyFeeFieldSpecified;
    
    private string exemptionReasonField;
    
    private string feeExemptField;
    
    private string paymentTypeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string accountNumber {
        get {
            return this.accountNumberField;
        }
        set {
            this.accountNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string attorneyClientRefNumber {
        get {
            return this.attorneyClientRefNumberField;
        }
        set {
            this.attorneyClientRefNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public decimal attorneyFee {
        get {
            return this.attorneyFeeField;
        }
        set {
            this.attorneyFeeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool attorneyFeeSpecified {
        get {
            return this.attorneyFeeFieldSpecified;
        }
        set {
            this.attorneyFeeFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string exemptionReason {
        get {
            return this.exemptionReasonField;
        }
        set {
            this.exemptionReasonField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string feeExempt {
        get {
            return this.feeExemptField;
        }
        set {
            this.feeExemptField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string paymentType {
        get {
            return this.paymentTypeField;
        }
        set {
            this.paymentTypeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class partyAlias {
    
    private string alternateNameField;
    
    private string alternateTypeCodeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string alternateName {
        get {
            return this.alternateNameField;
        }
        set {
            this.alternateNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string alternateTypeCode {
        get {
            return this.alternateTypeCodeField;
        }
        set {
            this.alternateTypeCodeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class name {
    
    private string firstNameField;
    
    private string lastNameField;
    
    private string middleNameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string firstName {
        get {
            return this.firstNameField;
        }
        set {
            this.firstNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string lastName {
        get {
            return this.lastNameField;
        }
        set {
            this.lastNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string middleName {
        get {
            return this.middleNameField;
        }
        set {
            this.middleNameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class address {
    
    private string addressLine1Field;
    
    private string addressLine2Field;
    
    private string cityField;
    
    private string stateCodeField;
    
    private string zipCodeField;
    
    private string zipCodeExtField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string addressLine1 {
        get {
            return this.addressLine1Field;
        }
        set {
            this.addressLine1Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string addressLine2 {
        get {
            return this.addressLine2Field;
        }
        set {
            this.addressLine2Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string city {
        get {
            return this.cityField;
        }
        set {
            this.cityField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string stateCode {
        get {
            return this.stateCodeField;
        }
        set {
            this.stateCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string zipCode {
        get {
            return this.zipCodeField;
        }
        set {
            this.zipCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string zipCodeExt {
        get {
            return this.zipCodeExtField;
        }
        set {
            this.zipCodeExtField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class party {
    
    private string accommodationTypeField;
    
    private string adaAccommodationIndField;
    
    private string additionalAccommodationDetailsField;
    
    private address addressField;
    
    private string corporationNameField;
    
    private string corporationTypeField;
    
    private string interpreterIndField;
    
    private string languageField;
    
    private name nameField;
    
    private string partyAffiliationField;
    
    private partyAlias[] partyAliasListField;
    
    private string partyDescriptionField;
    
    private string phoneNumberField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string accommodationType {
        get {
            return this.accommodationTypeField;
        }
        set {
            this.accommodationTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string adaAccommodationInd {
        get {
            return this.adaAccommodationIndField;
        }
        set {
            this.adaAccommodationIndField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string additionalAccommodationDetails {
        get {
            return this.additionalAccommodationDetailsField;
        }
        set {
            this.additionalAccommodationDetailsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public address address {
        get {
            return this.addressField;
        }
        set {
            this.addressField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string corporationName {
        get {
            return this.corporationNameField;
        }
        set {
            this.corporationNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string corporationType {
        get {
            return this.corporationTypeField;
        }
        set {
            this.corporationTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string interpreterInd {
        get {
            return this.interpreterIndField;
        }
        set {
            this.interpreterIndField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string language {
        get {
            return this.languageField;
        }
        set {
            this.languageField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public name name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string partyAffiliation {
        get {
            return this.partyAffiliationField;
        }
        set {
            this.partyAffiliationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("partyAliasList", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public partyAlias[] partyAliasList {
        get {
            return this.partyAliasListField;
        }
        set {
            this.partyAliasListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string partyDescription {
        get {
            return this.partyDescriptionField;
        }
        set {
            this.partyDescriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string phoneNumber {
        get {
            return this.phoneNumberField;
        }
        set {
            this.phoneNumberField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class @case {
    
    private string caseActionField;
    
    private string courtSectionField;
    
    private string defendantCaptionField;
    
    private decimal demandAmountField;
    
    private bool demandAmountFieldSpecified;
    
    private string docketDetailsForOtherCourtField;
    
    private string juryDemandField;
    
    private string lawFirmCaseIdField;
    
    private string otherCourtActionsField;
    
    private string plaintiffCaptionField;
    
    private string serviceMethodField;
    
    private string venueField;
    
    private string venueOfIncidentField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string caseAction {
        get {
            return this.caseActionField;
        }
        set {
            this.caseActionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string courtSection {
        get {
            return this.courtSectionField;
        }
        set {
            this.courtSectionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string defendantCaption {
        get {
            return this.defendantCaptionField;
        }
        set {
            this.defendantCaptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public decimal demandAmount {
        get {
            return this.demandAmountField;
        }
        set {
            this.demandAmountField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool demandAmountSpecified {
        get {
            return this.demandAmountFieldSpecified;
        }
        set {
            this.demandAmountFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string docketDetailsForOtherCourt {
        get {
            return this.docketDetailsForOtherCourtField;
        }
        set {
            this.docketDetailsForOtherCourtField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string juryDemand {
        get {
            return this.juryDemandField;
        }
        set {
            this.juryDemandField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string lawFirmCaseId {
        get {
            return this.lawFirmCaseIdField;
        }
        set {
            this.lawFirmCaseIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string otherCourtActions {
        get {
            return this.otherCourtActionsField;
        }
        set {
            this.otherCourtActionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string plaintiffCaption {
        get {
            return this.plaintiffCaptionField;
        }
        set {
            this.plaintiffCaptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string serviceMethod {
        get {
            return this.serviceMethodField;
        }
        set {
            this.serviceMethodField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string venue {
        get {
            return this.venueField;
        }
        set {
            this.venueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string venueOfIncident {
        get {
            return this.venueOfIncidentField;
        }
        set {
            this.venueOfIncidentField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class attachment {
    
    private byte[] bytesField;
    
    private string contentTypeField;
    
    private string docTypeField;
    
    private string documentCodeField;
    
    private string documentDescriptionField;
    
    private string documentNameField;
    
    private string extentionField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
    public byte[] bytes {
        get {
            return this.bytesField;
        }
        set {
            this.bytesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string contentType {
        get {
            return this.contentTypeField;
        }
        set {
            this.contentTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string docType {
        get {
            return this.docTypeField;
        }
        set {
            this.docTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string documentCode {
        get {
            return this.documentCodeField;
        }
        set {
            this.documentCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string documentDescription {
        get {
            return this.documentDescriptionField;
        }
        set {
            this.documentDescriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string documentName {
        get {
            return this.documentNameField;
        }
        set {
            this.documentNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string extention {
        get {
            return this.extentionField;
        }
        set {
            this.extentionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class bulkFilingPacket {
    
    private attachment[] attachmentListField;
    
    private string attorneyFirmIdField;
    
    private string attorneyIdField;
    
    private @case civilCaseField;
    
    private party[] defendantListField;
    
    private string documentRedactionIndField;
    
    private efilingNumber efilingNumberField;
    
    private fee feeField;
    
    private party[] plaintiffListField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("attachmentList", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public attachment[] attachmentList {
        get {
            return this.attachmentListField;
        }
        set {
            this.attachmentListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string attorneyFirmId {
        get {
            return this.attorneyFirmIdField;
        }
        set {
            this.attorneyFirmIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string attorneyId {
        get {
            return this.attorneyIdField;
        }
        set {
            this.attorneyIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public @case civilCase {
        get {
            return this.civilCaseField;
        }
        set {
            this.civilCaseField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("defendantList", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public party[] defendantList {
        get {
            return this.defendantListField;
        }
        set {
            this.defendantListField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string documentRedactionInd {
        get {
            return this.documentRedactionIndField;
        }
        set {
            this.documentRedactionIndField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public efilingNumber efilingNumber {
        get {
            return this.efilingNumberField;
        }
        set {
            this.efilingNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public fee fee {
        get {
            return this.feeField;
        }
        set {
            this.feeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("plaintiffList", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public party[] plaintiffList {
        get {
            return this.plaintiffListField;
        }
        set {
            this.plaintiffListField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
public partial class civilFilingRequest {
    
    private bulkFilingPacket bulkFilingPacketField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bulkFilingPacket bulkFilingPacket {
        get {
            return this.bulkFilingPacketField;
        }
        set {
            this.bulkFilingPacketField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/", IsNullable=false)]
public partial class Exception {
    
    private string messageField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string message {
        get {
            return this.messageField;
        }
        set {
            this.messageField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/", IsNullable=false)]
public partial class getCivilFilingStatus {
    
    private civilFilingRequest arg0Field;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public civilFilingRequest arg0 {
        get {
            return this.arg0Field;
        }
        set {
            this.arg0Field = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/", IsNullable=false)]
public partial class getCivilFilingStatusResponse {
    
    private civilFilingResponse returnField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public civilFilingResponse @return {
        get {
            return this.returnField;
        }
        set {
            this.returnField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/", IsNullable=false)]
public partial class submitCivilFiling {
    
    private civilFilingRequest arg0Field;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public civilFilingRequest arg0 {
        get {
            return this.arg0Field;
        }
        set {
            this.arg0Field = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://webservice.civilfiling.ecourts.ito.aoc.nj/", IsNullable=false)]
public partial class submitCivilFilingResponse {
    
    private civilFilingResponse returnField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public civilFilingResponse @return {
        get {
            return this.returnField;
        }
        set {
            this.returnField = value;
        }
    }
}
