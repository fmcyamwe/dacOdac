namespace Dac.Neo.Model; 

public class Doctor
{
    public int Id { get; set;} //tab for auto-completion

    public string? Name { get; set;}//huh weird those ?
    public string? Speciality { get; set;}

    //phone? or contact info...
    
    public int? Born { get; set;}  //PII >>needs Auth**
    public DateTime VisitDate { get; set;}

    //medical history(or) //DEF need auth**

    //emergency_contact (can access medical history too)

}