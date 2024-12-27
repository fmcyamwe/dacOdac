using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dac.Neo.Model; 

public class Patient(string first, string last)
{
    [JsonIgnore]
    [JsonPropertyName("id")] //annotation doesnt seem requiered with AddNewtonsoftJson...but just in case...
    public string? Id { get; set;} //using string instead of Neo4J's brittle IDs >> generated with GUID
    
    [Required]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = first;

    [Required]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = last;

    [JsonPropertyName("born")] //umm toReview** naming >> bornIn
    public int? Born { get; set;}  //PII >>needs Auth?
    
    //public string? Address { get; set;} //PII >>needs Auth too prolly...
    //phone? or contact info...
    //todo** if enough time for Address, phone and contact info...
    //emergency_contact (can access medical history too)
    public string? Gender { get; set;} //string or someting smaller like rune? >>ENUM!!! https://ardalis.com/enum-alternatives-in-c/

    //current doctor?

    [JsonIgnore]
    public Treatment? CurrentTreatment { get; set;}

    [JsonIgnore]
    public List<Treatment>? MedicalHistory { get; set; }//DEF need auth** //only viewable & editable by doctor

    [JsonIgnore] //does hide from schema
    public DateTime? VisitDate { get; set;} //or lastVisitDate?


//string json = JsonExtensions.FromJson(person);
}