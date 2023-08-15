using System.Text.Json.Serialization;

public class CimResource
{
    public string Kind { get; set; } = string.Empty;

    public string Self { get; set; } = string.Empty;

    public string Class { get; set; } = string.Empty;

    public IDictionary<string, object>? Properties { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? Methods { get; set; }
}
