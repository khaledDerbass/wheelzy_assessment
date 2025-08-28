// Sample code file for testing CodeFileProcessor
// This file contains various scenarios that should be processed

using System;
using System.Threading.Tasks;

namespace WheelzyAssessment.Tests.TestData;

public class SampleCode
{
    // Async method without Async suffix - should be fixed
    public async Task GetData()
    {
        await Task.Delay(100);
    }
    // Missing blank line above - should be fixed
    public async Task<string> ProcessInfo()
    {
        return await Task.FromResult("processed");
    }

    // Correct async method - should not be changed
    public async Task GetDataAsync()
    {
        await Task.Delay(100);
    }

    // ViewModel acronym issues - should be fixed
    public CustomerVm GetCustomerVm()
    {
        return new CustomerVm();
    }

    public List<OrderDto> GetOrderDtos()
    {
        return new List<OrderDto>();
    }

    // Mixed issues
    public async Task ProcessVms()
    {
        var vms = GetCustomerVms();
        // Process vms
    }
}

public class CustomerVm
{
    public string Name { get; set; } = "";
}

public class OrderDto  
{
    public int Id { get; set; }
}
