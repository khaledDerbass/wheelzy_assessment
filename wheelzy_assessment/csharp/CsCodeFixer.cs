// Roslyn-based .cs fixer. Requires Microsoft.CodeAnalysis.* packages.
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class CsCodeFixer
{
    public static async Task<int> ProcessFolderAsync(string rootFolder, CancellationToken ct = default)
    {
        var files = Directory.GetFiles(rootFolder, "*.cs", SearchOption.AllDirectories);
        int changed = 0;

        foreach (var path in files)
        {
            var text = await File.ReadAllTextAsync(path, ct);
            var updated = await ProcessTextAsync(text, ct);

            if (updated != text)
            {
                await File.WriteAllTextAsync(path, updated, ct);
                changed++;
            }
        }
        return changed;
    }

    public static async Task<string> ProcessTextAsync(string source, CancellationToken ct = default)
    {
        var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: ct);
        var root = await tree.GetRootAsync(ct);

        var rewriter = new FixRewriter();
        var newRoot = (CompilationUnitSyntax)rewriter.Visit(root);

        var updated = newRoot.ToFullString();

        // casing fixes for Vm/Dto -> VM/DTO etc.
        updated = Regex.Replace(updated, @"\b([A-Za-z0-9_]+?)Vm\b", "$1VM");
        updated = Regex.Replace(updated, @"\b([A-Za-z0-9_]+?)Vms\b", "$1VMs");
        updated = Regex.Replace(updated, @"\b([A-Za-z0-9_]+?)Dto\b", "$1DTO");
        updated = Regex.Replace(updated, @"\b([A-Za-z0-9_]+?)Dtos\b", "$1DTOs");

        // ensure blank line between methods
        updated = EnsureBlankLineBetweenMethods(updated);

        return updated;
    }

    private static string EnsureBlankLineBetweenMethods(string code)
    {
        var pattern = @"(\n\s*\}\s*\n)(\s*\[?\s*(public|private|protected|internal)\s+(static\s+)?(async\s+)?[A-Za-z0-9_<>,\[\]\?]+\s+[A-Za-z0-9_]+\s*\()";
        return Regex.Replace(code, pattern, m =>
        {
            var part1 = m.Groups[1].Value;
            var part2 = m.Groups[2].Value;
            if (part1.Contains("\n\n")) return m.Value;
            return part1 + "\n" + part2;
        });
    }

    private sealed class FixRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var updated = node;

            if (node.Modifiers.Any(SyntaxKind.AsyncKeyword))
            {
                var name = node.Identifier.Text;
                if (!name.EndsWith("Async", StringComparison.Ordinal))
                {
                    updated = node.WithIdentifier(SyntaxFactory.Identifier(name + "Async")
                        .WithTriviaFrom(node.Identifier));
                }
            }

            return base.VisitMethodDeclaration(updated);
        }
    }
}
