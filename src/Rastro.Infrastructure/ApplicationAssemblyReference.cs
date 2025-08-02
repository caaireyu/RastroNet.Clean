using System.Reflection;

namespace Rastro.Infrastructure;

public class ApplicationAssemblyReference
{
    internal static readonly Assembly Assembly = typeof(ApplicationAssemblyReference).Assembly;
}