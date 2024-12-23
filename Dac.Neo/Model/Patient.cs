namespace Dac.Neo.Model; 

public class Patient(string first, string last)
{
    public string? ID { get; set;} //was int but string instead of using Neo4J brittle IDs >>will generate with GUID
    public string FirstName { get; set; } = first;
    public string LastName { get; set; } = last;
    public string? Address { get; set;} //PII >>needs Auth too prolly...


    public string? Gender { get; set;} //string or someting smaller like rune? >>ENUM!!! https://ardalis.com/enum-alternatives-in-c/

    //public int Age { get; set;} //or DOB?...prolly better

    //phone? or contact info...
    
    public int? Born { get; set;}  //PII >>needs Auth**
    public DateTime VisitDate { get; set;}

    //medical history(or) //DEF need auth**

    //emergency_contact (can access medical history too)

}