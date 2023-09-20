namespace CimRs.Wmi.Models;

public class CimInstanceCollection
{
    public string Kind { get; set; } = "instancecollection";

    public string Self { get; set; } = string.Empty;

    public IList<CimResource> instances { get; set; } = new List<CimResource>();
}