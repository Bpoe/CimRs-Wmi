using System.Text.Json.Serialization;

public class CimPropertyDefinition
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, CimQualifier>? Qualifiers { get; set; }

    public object? Array { get; set; }

    public object? Type { get; set; }

    public object? ClassName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? DefaultValue { get; set; }
}
