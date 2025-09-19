// TODO eventually add something like this to generate non-generic versions of generic MonoBehaviour classes.
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
//
// namespace Grid.SourceGenerators
// {
//     [Generator]
//     public class MouseGridSelectorGenerator : ISourceGenerator
//     {
//         public void Initialize(GeneratorInitializationContext context)
//         {
//             context.RegisterForSyntaxNotifications(() => new CandidateReceiver());
//         }
//
//         public void Execute(GeneratorExecutionContext context)
//         {
//             if (context.SyntaxReceiver is not CandidateReceiver receiver)
//                 return;
//
//             var compilation = context.Compilation;
//
//             // Your runtime types live in Grid.Runtime assembly
//             var mouseGridSelectorSymbol = compilation.GetTypeByMetadataName("Grid.MouseGridSelector`1");
//             var iGridNodeSymbol = compilation.GetTypeByMetadataName("Grid.IGridNode`1");
//
//             if (mouseGridSelectorSymbol is null || iGridNodeSymbol is null)
//                 return;
//
//             foreach (var candidate in receiver.Candidates)
//             {
//                 var model = compilation.GetSemanticModel(candidate.SyntaxTree);
//                 if (model.GetDeclaredSymbol(candidate) is not INamedTypeSymbol symbol) 
//                     continue;
//
//                 // must implement IGridNode<T>
//                 if (!symbol.AllInterfaces.Any(i =>
//                         i.OriginalDefinition.Equals(iGridNodeSymbol, SymbolEqualityComparer.Default)))
//                     continue;
//
//                 var nodeName = symbol.Name;
//                 var ns = "Grid.Generated"; // 👈 all selectors go here
//
//                 var source = GenerateSelector(ns, nodeName, mouseGridSelectorSymbol);
//                 context.AddSource($"{nodeName}MouseGridSelector.g.cs", source);
//             }
//         }
//
//         private string GenerateSelector(string ns, string nodeName, INamedTypeSymbol baseSymbol)
//         {
//             var sb = new StringBuilder();
//             sb.AppendLine($"namespace {ns};");
//             sb.AppendLine();
//             sb.AppendLine($"public class {nodeName}MouseGridSelector : {baseSymbol.Name}<{nodeName}> {{ }}");
//             return sb.ToString();
//         }
//
//         private class CandidateReceiver : ISyntaxReceiver
//         {
//             public List<ClassDeclarationSyntax> Candidates { get; } = new();
//
//             public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
//             {
//                 if (syntaxNode is ClassDeclarationSyntax cds)
//                 {
//                     Candidates.Add(cds);
//                 }
//             }
//         }
//     }
// }
