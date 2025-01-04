using System.ComponentModel.DataAnnotations;

namespace Dac.Neo.Data.Model; 

public class Treatment //string name, string by
{
    public Treatment(){} //prolly gotta add neo4J annotations...
    //[Required]
    public string Name { get; set; } //= name;
    
    //[Required]
    public string By { get; set;} //= by; //doctor's Id 

    public string? Details { get; set;}
    
    public long StartDate { get; set;} //long instead of DateTime

    public long? EndDate { get; set;} //DateTime
}