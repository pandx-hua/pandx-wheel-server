using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using AsmResolver;
using pandx.Wheel.Modules;
using ModuleDefinition = AsmResolver.DotNet.ModuleDefinition;

namespace pandx.Wheel.Helpers;

public static class AssemblyHelper
{
    public static IEnumerable<Assembly> GetReferencedAssemblies()
    {
        var rootAssembly = Assembly.GetEntryAssembly();
        if (rootAssembly is null)
        {
            rootAssembly = Assembly.GetCallingAssembly();
        }

        var assemblies = new HashSet<Assembly>(new AssemblyEquality());
        var loaded = new HashSet<string>();
        var queue = new Queue<Assembly>();
        queue.Enqueue(rootAssembly);
        if (IsSystemAssembly(rootAssembly))
        {
            if (IsValid(rootAssembly))
            {
                assemblies.Add(rootAssembly);
            }
        }

        while (queue.Any())
        {
            var assemblyToCheck = queue.Dequeue();
            foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
            {
                if (!loaded.Contains(reference.FullName))
                {
                    var assembly = Assembly.Load(reference);
                    if (IsSystemAssembly(assembly))
                    {
                        continue;
                    }

                    queue.Enqueue(assembly);
                    loaded.Add(reference.FullName);
                    if (IsValid(assembly))
                    {
                        assemblies.Add(assembly);
                    }
                }
            }
        }

        var files = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll",
            new EnumerationOptions { RecurseSubdirectories = true });
        foreach (var file in files)
        {
            if (!IsManagedAssembly(file))
            {
                continue;
            }

            var assemblyName = AssemblyName.GetAssemblyName(file);
            if (assemblies.Any(a => AssemblyName.ReferenceMatchesDefinition(a.GetName(), assemblyName)))
            {
                continue;
            }

            if (IsSystemAssembly(file))
            {
                continue;
            }

            var assembly = TryLoadAssembly(file);
            if (assembly is null)
            {
                continue;
            }

            if (!IsValid(assembly))
            {
                continue;
            }

            if (IsSystemAssembly(assembly))
            {
                continue;
            }

            assemblies.Add(assembly);
        }

        return assemblies.ToArray();
    }

    private static bool IsValid(Assembly assembly)
    {
        try
        {
            var types = assembly.GetTypes().Where(t =>
                t is { IsClass: true, IsAbstract: false } && typeof(IModule).IsAssignableFrom(t));
            if (!types.Any())
            {
                return false;
            }

            var definedTypes = assembly.DefinedTypes.ToList();
            return true;
        }
        catch (ReflectionTypeLoadException)
        {
            return false;
        }
    }

    private static bool IsSystemAssembly(Assembly assembly)
    {
        var companyAttribute = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
        if (companyAttribute is null)
        {
            return false;
        }

        var companyName = companyAttribute.Company;
        return companyName.Contains("Microsoft");
    }

    private static Assembly? TryLoadAssembly(string file)
    {
        var assemblyName = AssemblyName.GetAssemblyName(file);
        Assembly? assembly = null;
        try
        {
            assembly = Assembly.Load(assemblyName);
        }
        catch
        {
            // ignored
        }

        if (assembly is null)
        {
            try
            {
                assembly = Assembly.LoadFile(file);
            }
            catch
            {
                // ignored
            }
        }

        return assembly;
    }

    private static bool IsSystemAssembly(string path)
    {
        var moduleDefinition = ModuleDefinition.FromFile(path);
        var assembly = moduleDefinition.Assembly;
        if (assembly is null)
        {
            return false;
        }

        var companyAttribute = assembly.CustomAttributes.FirstOrDefault(a =>
            a.Constructor?.DeclaringType?.FullName == typeof(AssemblyCompanyAttribute).FullName);
        if (companyAttribute is null)
        {
            return false;
        }

        var companyName = ((Utf8String?)companyAttribute.Signature?.FixedArguments[0].Element)?.Value;

        if (companyName is null)
        {
            return false;
        }

        return companyName.Contains("Microsoft");
    }

    private static bool IsManagedAssembly(string file)
    {
        using var fileStream = File.OpenRead(file);
        using var peReader = new PEReader(fileStream);
        return peReader.HasMetadata && peReader.GetMetadataReader().IsAssembly;
    }

    private class AssemblyEquality : EqualityComparer<Assembly>
    {
        public override bool Equals(Assembly? x, Assembly? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return AssemblyName.ReferenceMatchesDefinition(x.GetName(), y.GetName());
        }

        public override int GetHashCode([DisallowNull] Assembly obj)
        {
            return obj.GetName().FullName.GetHashCode();
        }
    }
}