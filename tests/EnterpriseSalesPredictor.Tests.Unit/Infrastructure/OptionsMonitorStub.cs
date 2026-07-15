using Microsoft.Extensions.Options;

namespace EnterpriseSalesPredictor.Tests.Unit.Infrastructure;

internal sealed class OptionsMonitorStub<TOptions> : IOptionsMonitor<TOptions>
{
    public OptionsMonitorStub(TOptions currentValue)
    {
        CurrentValue = currentValue;
    }

    public TOptions CurrentValue { get; }

    public TOptions Get(string? name) => CurrentValue;

    public IDisposable? OnChange(Action<TOptions, string?> listener) => null;
}
