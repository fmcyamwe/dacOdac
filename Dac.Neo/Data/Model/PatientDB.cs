using System.ComponentModel.DataAnnotations;
//using System.Text.Json.Serialization;
using Neo4j.Driver.Extensions;


namespace Dac.Neo.Data.Model;

//umm naming? toReview***
public class PatientDB //<T> where T : new()//: ILookupDataResponse<PatientDB>
{
    /*public Patient(string first, string last)
    {
        FirstName = first;
        LastName = last;
    }*/
    public PatientDB(){}   //class mapped to Neo4J Node for parsing

    [Neo4jProperty(Name = "id")] //Required
    public string? Id { get; set;} //using string instead of Neo4J's brittle IDs >> generated with GUID
    
    //[Required]
    //[JsonPropertyName("firstName")]
    [Neo4jProperty(Name = "firstName")] //Required
    public string? FirstName { get; set; } //required

    [Neo4jProperty(Name = "lastName")] 
    public string? LastName { get; set; } //required OR [Required] >>the latter---toUse**

    //[JsonPropertyName("born")] 
    [Neo4jProperty(Name = "born")] //umm toReview** naming >> bornIn
    public int? Born { get; set;}  //PII >>needs Auth?
    
    //public string? Address { get; set;} //PII >>needs Auth too prolly...
    //phone? or contact info...
    //todo** if enough time for Address, phone and contact info...
    //emergency_contact (can access medical history too)

    [Neo4jProperty(Name = "gender")] //(Ignore = true) ?? ignore a property, so no attempt is made to read it
    public string? Gender { get; set;} //string or someting smaller like rune? >>ENUM!!! https://ardalis.com/enum-alternatives-in-c/

}