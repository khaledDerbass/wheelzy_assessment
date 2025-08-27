using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

public class CsCodeFixerTests
{
    [Fact]
    public async Task Renames_Async_Methods_Without_Suffix()
    {
        var src = @"class C { public async System.Threading.Tasks.Task DoWork(){ await System.Threading.Tasks.Task.Delay(1); } }";

        var result = await CsCodeFixer.ProcessTextAsync(src);
        result.Should().Contain("DoWorkAsync");
        result.Should().NotContain("DoWork(){");
    }

    [Theory]
    [InlineData("UserVm", "UserVM")]
    [InlineData("UsersVms", "UsersVMs")]
    [InlineData("OrderDto", "OrderDTO")]
    [InlineData("OrdersDtos", "OrdersDTOs")]
    public async Task Casing_Fixes_For_Vm_Dto(string before, string after)
    {
        var src = $"class C {{ {before} a; }}";
        var result = await CsCodeFixer.ProcessTextAsync(src);
        result.Should().Contain(after);
    }

    [Fact]
    public async Task Adds_Blank_Line_Between_Methods()
    {
        var src = @"class C { void A() { } void B() { } }";

        var result = await CsCodeFixer.ProcessTextAsync(src);
        // Expect one extra blank line between } and next signature
        result.Should().MatchRegex("\\}\\s*\\n\\s*\\n\\s*void B");
    }
}
