using System;
using Xunit;

namespace EndToEnd.Tests;

/// <summary>
/// Custom fact attribute that only runs when RUN_E2E_TESTS=true is set.
/// Prevents CI/local runs from failing when the frontend/API are not running.
/// </summary>
public sealed class E2EFactAttribute : FactAttribute
{
    private static readonly bool Enabled = string.Equals(
        Environment.GetEnvironmentVariable("RUN_E2E_TESTS"),
        "true",
        StringComparison.OrdinalIgnoreCase);

    public E2EFactAttribute()
    {
        if (!Enabled)
        {
            Skip = "E2E tests are disabled. Set RUN_E2E_TESTS=true to enable them.";
        }
    }
}

