namespace CimRs.Wmi.Models;

using System.Text.Json.Serialization;

public class CimMethodDefinition
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, CimQualifier>? Qualifiers { get; set; }

    public string? Type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClassName { get; set; }
}

//    {
//        "qualifiers": {
//            "qualifierName": {
//                "array": (qualifier - array),?
//                "type": (qualifier - type),
//                "value": (qualifier - value)
//            }#
//        },?
//        "type": (method -return -type),
//        "classname": (method -return -classname)?
//        "parameters": {
//            "parameterName": {
//                    "qualifiers": {
//                        "qualifierName": {
//                            "array": (qualifier - array),?
//                            "type": (qualifier - type),
//                            "value": (qualifier - value)
//                            }#
//                    },?
//                    "array": (parameter - array),?
//                    "arraysize": (parameter - arraysize),?
//                    "type": (parameter - type),
//                    "classname": (parameter - classname) ?
//                }#
//            }?
//        }#
//    }