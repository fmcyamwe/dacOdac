using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
//using System.Text.Json.Serialization;

namespace Dac.API.Model; 
public record PaginationRequest(
    [property: Description("Number of items to return in a single page of results")]
    [property: DefaultValue(10)]
    int PageSize = 10,

    [property: Description("The index of the page of results to return")]
    [property: DefaultValue(0)]
    int PageIndex = 0
);

//toSee if should rename to ActionRequest (to handle multiple 'action' types)
public record VisitRequest(
    [property: Description("The doctor's Id")]
    //[property: Required()] //borks when this present...coliiis
    //[JsonPropertyName("id"), JsonIgnore]
    string DoctorId,

    [property: Description("Short Reason for Request")]
    [property: DefaultValue("")]
    string Reason, //no complain here as below

    [property: Description("The type of request made by Patient")]
    [property: DefaultValue("")]
    string Action
);

/*
{
    "doctorId": "266721c1816c",
    "reason": "Routine check-up",
    "action": "visit"
}

*/