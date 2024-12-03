using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.CodeAnalysis;

public static class TestExtensions
{
    public static TAnalyzerTest WithPreprocessorSymbols<TAnalyzerTest>(this TAnalyzerTest test, params string[] symbols)
        where TAnalyzerTest : AnalyzerTest<DefaultVerifier>
    {
        test.OptionsTransforms.Add(options =>
        {
            return options;
        });

        test.SolutionTransforms.Add((solution, projectId) =>
        {
            var project = solution.GetProject(projectId);
            var parseOptions = (CSharpParseOptions)project!.ParseOptions!;

            parseOptions = parseOptions.WithPreprocessorSymbols(
                symbols.Length > 0 ? symbols : ["DDI_ADDSERVICE", "DDI_ADDSERVICES"]);

            return solution.WithProjectParseOptions(projectId, parseOptions);
        });

        return test;
    }
}
