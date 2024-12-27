using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dac.Neo.Model; 

public class Doctor
{
    [JsonPropertyName("id"), JsonIgnore]
    public string? Id { get; set;} //annoying tab for auto-completion

    //could be skipped and default to 'Dr.'
    [JsonPropertyName("firstName")] public string? FirstName { get; set;}

    [Required]
    [JsonPropertyName("lastName")]
    public required string LastName { get; set;}

    [JsonPropertyName("speciality")]
    public string? Speciality { get; set;}  //requiered?

    //phone? or contact info...
    [JsonPropertyName("practising")]
    public DateTime? PractisingSince { get; set;} //requiered?

}