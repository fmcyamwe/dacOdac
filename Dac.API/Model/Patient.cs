using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Dac.Neo.Data.Model;

namespace Dac.API.Model; 
public class Patient
{
    /*public Patient(string first, string last)
    {
        FirstName = first;
        LastName = last;
    }*/
    public Patient(){}

    [JsonPropertyName("id"), JsonIgnore] //annotation doesnt seem requiered with AddNewtonsoftJson...but just in case...
    public string? Id { get; set;} //here should be requiered? toReview**
    //using string instead of Neo4J's brittle IDs >> generated with GUID
    
    [Required]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } //required

    [Required]
    [JsonPropertyName("lastName")]
    public  string LastName { get; set; } //required

    [JsonPropertyName("born")] //umm toReview** naming >> bornIn
    public int? Born { get; set;}  //PII >>needs Auth?
    
    //public string? Address { get; set;} //PII >>needs Auth too prolly...
    //phone? or contact info...
    //todo** if enough time for Address, phone and contact info...
    //emergency_contact (can access medical history too)
    [JsonPropertyName("gender")]
    public string? Gender { get; set;} //string or someting smaller like rune? >>ENUM!!! https://ardalis.com/enum-alternatives-in-c/

    public List<AttendingDoctor>? AttendingDoctors { get; set;} //current doctor //could be multiple?

    //[JsonIgnore] //Name = "treatment")]
    public Treatment? CurrentTreatment { get; set;}  //could also be multiple..toreview**

/* //todo** re-enable below
   
    [JsonIgnore]
    [Neo4jProperty(Ignore = true)]
    public List<Treatment>? MedicalHistory { get; set; }//DEF need auth** //only viewable & editable by doctor

    [JsonIgnore] //does hide from schema
    [Neo4jProperty(Ignore = true)]
    public DateTime? VisitDate { get; set;} //or lastVisitDate?
*/

//string json = JsonExtensions.FromJson(person);
}