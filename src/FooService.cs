using System;
using System.Diagnostics;

namespace ReactiveStock;

/// <summary>
/// Nonsensical class to demonstrate an disposable service.
/// </summary>
internal class FooService : IDisposable
{
    public void Dispose()
    {
        Debug.WriteLine("FooService.Dispose");
    }
}