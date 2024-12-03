using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Devlooped.Sponsors;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using static Devlooped.Sponsors.SponsorLink;

namespace Devlooped.Extensions.DependencyInjection;

[Generator(LanguageNames.CSharp)]
public class StaticGenerator : ISourceGenerator
{
    public const string DefaultNamespace = "Microsoft.Extensions.DependencyInjection";
    public const string DefaultAddServicesClass = nameof(AddServicesNoReflectionExtension);

    public static string AddServicesExtension => ThisAssembly.Resources.AddServicesNoReflectionExtension.Text;
    public static string ServiceAttribute => ThisAssembly.Resources.ServiceAttribute.Text;
    public static string ServiceAttributeT => ThisAssembly.Resources.ServiceAttribute_1.Text;

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(c =>
        {
            c.AddSource("ServiceAttribute.g", ThisAssembly.Resources.ServiceAttribute.Text);
            c.AddSource("ServiceAttribute`1.g", ThisAssembly.Resources.ServiceAttribute_1.Text);
        });
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var rootNs = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.AddServicesNamespace", out var value) && !string.IsNullOrEmpty(value)
            ? value : DefaultNamespace;

        var className = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.AddServicesClassName", out value) && !string.IsNullOrEmpty(value) ?
            value : DefaultAddServicesClass;

        var code = ThisAssembly.Resources.AddServicesNoReflectionExtension.Text
            .Replace("namespace " + DefaultNamespace, "namespace " + rootNs)
            .Replace(DefaultAddServicesClass, className);

        if (IsEditor)
        {
            var status = Diagnostics.GetOrSetStatus(context.GetStatusOptions());
            string? remarks = default;
            string? warn = default;

            if (status == SponsorStatus.Unknown || status == SponsorStatus.Expired)
            {
                warn =
                    $"""
                    [Obsolete("{string.Format(CultureInfo.CurrentCulture, Resources.Editor_Disabled, Funding.Product, Funding.HelpUrl)}", false
                    #if NET6_0_OR_GREATER
                        , UrlFormat = "{Funding.HelpUrl}"
                    #endif
                    )]
                    """;

                remarks = Resources.Editor_DisabledRemarks;
            }
            else if (status == SponsorStatus.Grace && Diagnostics.TryGet() is { } grace && grace.Properties.TryGetValue(nameof(SponsorStatus.Grace), out var days))
            {
                remarks = string.Format(CultureInfo.CurrentCulture, Resources.Editor_GraceRemarks, days);
            }

            if (remarks != null)
            {
                // Remove /// <remarks> and /// </remarks> LINES from the remarks string
                var builder = new StringBuilder();
                foreach (var line in ReadLines(remarks))
                {
                    if (line.EndsWith("/// <remarks>") || line.EndsWith("/// </remarks>"))
                        continue;
                    if (line.TrimStart() is { Length: > 0 } trimmed && trimmed.StartsWith("///"))
                        builder.AppendLine(trimmed);
                }
                remarks = builder.AppendLine("///").ToString();
            }

            if (remarks != null || warn != null)
            {
                var builder = new StringBuilder();
                foreach (var line in ReadLines(code))
                {
                    if (remarks != null && line.EndsWith("/// <remarks>"))
                    {
                        builder.AppendLine(line);
                        // trim the remarks line to remove leading spaces and 
                        // replace them with the indenting from the target code line
                        var indent = line.IndexOf("/// <remarks>");
                        foreach (var rline in ReadLines(remarks))
                        {
                            builder.Append(new string(' ', indent)).AppendLine(rline);
                        }
                    }
                    else if (warn != null && line.EndsWith("[DDIAddServices]"))
                    {
                        builder.AppendLine(line);
                        // trim the remarks line to remove leading spaces and 
                        // replace them with the indenting from the target code line
                        var indent = line.IndexOf("[DDIAddServices]");
                        // append indentation and the warning, also splitting lines and trimming start
                        foreach (var wline in ReadLines(warn))
                        {
                            builder.Append(new string(' ', indent)).AppendLine(wline.TrimStart());
                        }
                    }
                    else
                    {
                        builder.AppendLine(line);
                    }
                }
                code = builder.ToString();
            }
        }

        context.AddSource(DefaultAddServicesClass + ".g", code);
    }

    static IEnumerable<string> ReadLines(string text)
    {
        using var reader = new StringReader(text);
        string? line;
        while ((line = reader.ReadLine()) != null)
            yield return line;
    }
}
