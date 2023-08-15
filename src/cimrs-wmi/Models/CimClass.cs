namespace CimRs.Wmi.Models;

using System.Text.Json.Serialization;

public class CimClass
{
    public string Kind { get; set; } = "class";

    public string Self { get; set; } = string.Empty;

    public string Namespace { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string SuperClassName { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, CimQualifier>? Qualifiers { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, CimPropertyDefinition>? Properties { get; set; }


    //"methods": {
    //    "methodName": {
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
    //    }?
    //}

}