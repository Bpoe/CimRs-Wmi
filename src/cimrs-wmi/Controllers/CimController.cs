namespace CimRs.Wmi.Controllers;

using CimRs.Wmi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Management.Infrastructure;

[ApiController]
[Route(CimController.Prefix)]
public class CimController : ControllerBase, IDisposable
{
    public const string Prefix = "cim";

    private readonly CimSession cimSession = CimSession.Create(null);
    private bool disposedValue;

    [HttpGet("{cimNamespace}/classes")]
    public ActionResult<CimClassCollection> EnumerateClasses(string cimNamespace)
    {
        var response = new CimClassCollection
        {
            Self = $"{CimController.Prefix}/{cimNamespace}/classes",
            Classes = this.cimSession
                .EnumerateClasses(Uri.UnescapeDataString(cimNamespace))
                .Select(ModelExtensions.ToModel)
                .ToList(),
        };

        return this.Ok(response);
    }

    [HttpGet("{cimNamespace}/classes/{cimClass}")]
    public ActionResult<Models.CimClass> GetClass(string cimNamespace, string cimClass)
    {
        try
        {
            return this.Ok(this.cimSession
                .GetClass(Uri.UnescapeDataString(cimNamespace), cimClass)
                .ToModel());
        }
        catch (CimException ex) when (ex.MessageId == "HRESULT 0x80041002")
        {
            return this.NotFound();
        }
    }

    [HttpGet("{cimNamespace}/classes/{cimClass}/instances")]
    public ActionResult<CimInstanceCollection> EnumerateInstances(string cimNamespace, string cimClass)
    {
        var response = new CimInstanceCollection
        {
            Self = $"{CimController.Prefix}/{cimNamespace}/classes/{cimClass}/instances",
            instances = this.cimSession
                .EnumerateInstances(Uri.UnescapeDataString(cimNamespace), cimClass)
                .Select(ModelExtensions.ToModel)
                .ToList(),
        };

        return this.Ok(response);
    }

    [HttpGet("{cimNamespace}/classes/{cimClass}/instances/{id}")]
    public ActionResult<CimResource> GetInstance(string cimNamespace, string cimClass, string id)
    {
        var keys = GetKeys(id);
        try
        {
            using var instanceId = this.GetCimInstanceId(Uri.UnescapeDataString(cimNamespace), cimClass, keys);

            return this.cimSession
                .GetInstance(Uri.UnescapeDataString(instanceId.CimSystemProperties.Namespace), instanceId)
                .ToModel();
        }
        catch (CimException ex) when (ex.MessageId == "HRESULT 0x80041002")
        {
            return this.NotFound();
        }
    }

    [HttpPut("{cimNamespace}/classes/{cimClassName}/instances/{id}")]
    public ActionResult<CimResource> PutInstance(string cimNamespace, string cimClassName, string id, [FromBody] CimResource resource)
    {
        var keys = GetKeys(id);
        using var instanceId = this.GetCimInstanceId(Uri.UnescapeDataString(cimNamespace), cimClassName, keys);

        CimInstance? existingInstance;
        try
        {
            existingInstance = this.cimSession
                .GetInstance(instanceId.CimSystemProperties.Namespace, instanceId);
        }
        catch (CimException ex) when (ex.MessageId == "HRESULT 0x80041002")
        {
            existingInstance = null;
        }

        if (existingInstance is not null)
        {
            foreach (var p in resource.Properties)
            {
                if (existingInstance.CimInstanceProperties[p.Key].Flags.HasFlag(CimFlags.Key)
                    || existingInstance.CimInstanceProperties[p.Key].Flags.HasFlag(CimFlags.ReadOnly))
                {
                    continue;
                }

                var json = p.Value?.ToString();
                existingInstance.CimInstanceProperties[p.Key].Value = json is null
                    ? null
                    : existingInstance.CimClass.CimClassProperties[p.Key].CimType.FromString(json);
            }

            this.cimSession.ModifyInstance(existingInstance);

            return this.Ok(existingInstance.ToModel());
        }

        var cimClassProperties = this.cimSession.GetClass(Uri.UnescapeDataString(cimNamespace), cimClassName).CimClassProperties;

        foreach (var p in resource.Properties)
        {
            if (instanceId.CimInstanceProperties.Any(prop => prop.Name == p.Key))
            {
                continue;
            }

            var json = p.Value?.ToString();
            instanceId.CimInstanceProperties.Add(
                CimProperty.Create(
                    p.Key,
                    cimClassProperties[p.Key].CimType.FromString(json),
                    CimFlags.None));
        }

        var createdInstance = this.cimSession.CreateInstance(Uri.UnescapeDataString(cimNamespace), instanceId);

        return this.Ok(createdInstance.ToModel());
    }

    [HttpDelete("{cimNamespace}/classes/{cimClassName}/instances/{id}")]
    public ActionResult<CimResource> DeleteInstance(string cimNamespace, string cimClassName, string id)
    {
        var keys = GetKeys(id);
        using var instanceId = this.GetCimInstanceId(Uri.UnescapeDataString(cimNamespace), cimClassName, keys);

        CimInstance? existingInstance;
        try
        {
            existingInstance = this.cimSession
                .GetInstance(instanceId.CimSystemProperties.Namespace, instanceId);
        }
        catch (CimException ex) when (ex.MessageId == "HRESULT 0x80041002")
        {
            return this.NotFound();
        }

        this.cimSession.DeleteInstance(existingInstance);

        return this.NoContent();
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

    private CimInstance GetCimInstanceId(string cimNamespace, string cimClassName, IDictionary<string, string> keys)
    {
        var cimClassProperties = this.cimSession.GetClass(cimNamespace, cimClassName).CimClassProperties;

        var instanceId = new CimInstance(cimClassName, cimNamespace);
        foreach (var key in keys)
        {
            instanceId.CimInstanceProperties.Add(
                CimProperty.Create(
                    key.Key,
                    cimClassProperties[key.Key].CimType.FromString(key.Value),
                    CimFlags.Key));
        }

        return instanceId;
    }
}
