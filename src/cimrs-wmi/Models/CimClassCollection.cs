namespace CimRs.Wmi.Models;

public class CimClassCollection
{
    public string Kind { get; set; } = "classcollection";

    public string Self { get; set; } = string.Empty;

    public IList<CimClass> Classes { get; set; } = new List<CimClass>();
}
