using System.Text.RegularExpressions;

namespace WheelzyAssessment.Services;

public class CodeFileProcessor
{
    public async Task ProcessCodeFilesAsync(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

        var csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
        
        foreach (var filePath in csFiles)
        {
            await ProcessFileAsync(filePath);
        }
    }

    private async Task ProcessFileAsync(string filePath)
    {
        var content = await File.ReadAllTextAsync(filePath);
        var originalContent = content;

        // Apply all transformations
        content = FixAsyncMethodNames(content);
        content = FixViewModelAcronyms(content);
        content = AddBlankLinesBetweenMethods(content);

        // Only write if content changed
        if (content != originalContent)
        {
            await File.WriteAllTextAsync(filePath, content);
        }
    }

    public string FixAsyncMethodNames(string content)
    {
        // Match async methods without "Async" suffix
        var pattern = @"(?:public|private|protected|internal)\s+(?:static\s+)?async\s+Task(?:<[^>]+>)?\s+(\w+)\s*\(";
        
        return Regex.Replace(content, pattern, match =>
        {
            var methodName = match.Groups[1].Value;
            if (!methodName.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
            {
                var newMethodName = methodName + "Async";
                return match.Value.Replace(methodName, newMethodName);
            }
            return match.Value;
        });
    }

    public string FixViewModelAcronyms(string content)
    {
        var replacements = new Dictionary<string, string>
        {
            { @"\bVm\b", "VM" },
            { @"\bVms\b", "VMs" },
            { @"\bDto\b", "DTO" },
            { @"\bDtos\b", "DTOs" }
        };

        foreach (var replacement in replacements)
        {
            content = Regex.Replace(content, replacement.Key, replacement.Value);
        }

        return content;
    }

    public string AddBlankLinesBetweenMethods(string content)
    {
        // Match method declarations and ensure blank line before them
        var pattern = @"(\r?\n)([ \t]*(?:public|private|protected|internal)(?:\s+static)?\s+(?:\w+(?:<[^>]*>)?|\w+\[\])\s+\w+\s*\([^)]*\)\s*(?:\r?\n)*\s*\{)";
        
        return Regex.Replace(content, pattern, match =>
        {
            var lineBreak = match.Groups[1].Value;
            var methodDeclaration = match.Groups[2].Value;
            
            // Check if there's already a blank line
            if (!match.Value.StartsWith(lineBreak + lineBreak))
            {
                return lineBreak + lineBreak + methodDeclaration;
            }
            
            return match.Value;
        });
    }
}