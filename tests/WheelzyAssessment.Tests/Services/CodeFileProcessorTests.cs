using Microsoft.VisualStudio.TestTools.UnitTesting;
using WheelzyAssessment.Services;

namespace WheelzyAssessment.Tests.Services;

[TestClass]
public class CodeFileProcessorTests
{
    private CodeFileProcessor _processor = null!;

    [TestInitialize]
    public void Setup()
    {
        _processor = new CodeFileProcessor();
    }

    [TestMethod]
    public void FixAsyncMethodNames_ShouldAddAsyncSuffix_WhenMissing()
    {
        // Arrange
        var input = "public async Task GetData() { }";
        var expected = "public async Task GetDataAsync() { }";

        // Act
        var result = _processor.FixAsyncMethodNames(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void FixAsyncMethodNames_ShouldNotChange_WhenAsyncSuffixExists()
    {
        // Arrange
        var input = "public async Task GetDataAsync() { }";

        // Act
        var result = _processor.FixAsyncMethodNames(input);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void FixAsyncMethodNames_ShouldHandleGenericReturnTypes()
    {
        // Arrange
        var input = "public async Task<string> GetValue() { }";
        var expected = "public async Task<string> GetValueAsync() { }";

        // Act
        var result = _processor.FixAsyncMethodNames(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void FixAsyncMethodNames_ShouldHandlePrivateMethods()
    {
        // Arrange
        var input = "private async Task ProcessData() { }";
        var expected = "private async Task ProcessDataAsync() { }";

        // Act
        var result = _processor.FixAsyncMethodNames(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void FixViewModelAcronyms_ShouldReplaceVm_WithVM()
    {
        // Arrange
        var input = "CustomerVm model = new CustomerVm();";
        var expected = "CustomerVM model = new CustomerVM();";

        // Act
        var result = _processor.FixViewModelAcronyms(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void FixViewModelAcronyms_ShouldReplaceDto_WithDTO()
    {
        // Arrange
        var input = "List<CustomerDto> customers = GetCustomerDtos();";
        var expected = "List<CustomerDTO> customers = GetCustomerDTOs();";

        // Act
        var result = _processor.FixViewModelAcronyms(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void FixViewModelAcronyms_ShouldReplaceVms_WithVMs()
    {
        // Arrange
        var input = "var customerVms = GetAllVms();";
        var expected = "var customerVMs = GetAllVMs();";

        // Act
        var result = _processor.FixViewModelAcronyms(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void AddBlankLinesBetweenMethods_ShouldAddBlankLine_WhenMissing()
    {
        // Arrange
        var input = @"public void Method1() { }
public void Method2() { }";
        
        var expected = @"public void Method1() { }

public void Method2() { }";

        // Act
        var result = _processor.AddBlankLinesBetweenMethods(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void AddBlankLinesBetweenMethods_ShouldNotChange_WhenBlankLineExists()
    {
        // Arrange
        var input = @"public void Method1() { }

public void Method2() { }";

        // Act
        var result = _processor.AddBlankLinesBetweenMethods(input);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void AddBlankLinesBetweenMethods_ShouldHandleMultipleMethods()
    {
        // Arrange
        var input = @"public void Method1() { }
private string Method2() { return """"; }
protected async Task Method3() { }";

        var expected = @"public void Method1() { }

private string Method2() { return """"; }

protected async Task Method3() { }";

        // Act
        var result = _processor.AddBlankLinesBetweenMethods(input);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
