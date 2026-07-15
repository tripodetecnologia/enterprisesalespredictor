using EnterpriseSalesPredictor.Domain.Rules;

namespace EnterpriseSalesPredictor.Tests.Unit.Domain;

public sealed class ForecastRulesTests
{
    [TestCase(1, true)]
    [TestCase(365, true)]
    [TestCase(0, false)]
    [TestCase(366, false)]
    public void IsValidForecastRange_ShouldValidateConfiguredBounds(int horizonDays, bool expected)
    {
        var from = new DateTime(2026, 1, 1);
        var to = from.AddDays(horizonDays - 1);

        var result = ForecastRules.IsValidForecastRange(from, to);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void IsValidForecastRange_ShouldRejectReversedDates()
    {
        var result = ForecastRules.IsValidForecastRange(new DateTime(2026, 2, 2), new DateTime(2026, 2, 1));

        Assert.That(result, Is.False);
    }
}
