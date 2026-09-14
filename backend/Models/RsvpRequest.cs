namespace Hegerovi.Backend.Models;

public class RsvpRequest
{
    public bool Attending { get; set; }
    public int PeopleCount { get; set; }
    public string? MenuPreference { get; set; }
    public string? Allergies { get; set; }
}
