namespace CimRs.Wmi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Management.Infrastructure;

[ApiController]
[Route(CimController.Prefix)]
public class CimController : ControllerBase, IDisposable
{
    public const string Prefix = "cim";

    private readonly CimSession cimSession = CimSession.Create("localhost");
    private bool disposedValue;

    [HttpGet("{cimNamespace}/classes")]
    public IEnumerable<Models.CimClass> EnumerateClasses(string cimNamespace)
    => this.cimSession
        .EnumerateClasses(Uri.UnescapeDataString(cimNamespace))
        .Select(ModelExtensions.ToModel);

    [HttpGet("{cimNamespace}/classes/{cimClass}")]
    public Models.CimClass GetClass(string cimNamespace, string cimClass)
        => this.cimSession
            .GetClass(Uri.UnescapeDataString(cimNamespace), cimClass)
            .ToModel();

    [HttpGet("{cimNamespace}/classes/{cimClass}/instances")]
    public IEnumerable<CimResource> EnumerateInstances(string cimNamespace, string cimClass)
        => this.cimSession
            .EnumerateInstances(Uri.UnescapeDataString(cimNamespace), cimClass)
            .Select(ModelExtensions.ToModel);

    [HttpGet("{cimNamespace}/classes/{cimClass}/instances/{id}")]
    public CimResource GetInstance(string cimNamespace, string cimClass, string id)
    {
        var keys = GetKeys(id);
        using var instanceId = this.GetCimInstanceId(cimNamespace, cimClass, keys);

        return this.cimSession
            .GetInstance(Uri.UnescapeDataString(instanceId.CimSystemProperties.Namespace), instanceId)
            .ToModel();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.cimSession.Dispose();
            }

            disposedValue = true;
        }
    }

    private static IDictionary<string, string> GetKeys(string id)
    {
        return id
            .Split(',')
            .ToDictionary(
                k => k.Split('=')[0],
                k => k.Split('=')[1]);
    }

    private CimInstance GetCimInstanceId(string cimNamespace, string cimClass, IDictionary<string, string> keys)
    {
        var classProperties = this.cimSession
            .GetClass(Uri.UnescapeDataString(cimNamespace), cimClass)
            .CimClassProperties;

        var instanceId = new CimInstance(cimClass, cimNamespace);
        foreach (var key in keys)
        {
            var property = CimProperty.Create(
                key.Key,
                classProperties[key.Key].CimType.FromString(key.Value),
                CimFlags.Key);

            instanceId.CimInstanceProperties.Add(property);
        }

        return instanceId;
    }
}
