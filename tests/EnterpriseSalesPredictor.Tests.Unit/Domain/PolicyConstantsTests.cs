using EnterpriseSalesPredictor.Domain.Rules;

namespace EnterpriseSalesPredictor.Tests.Unit.Domain;

public sealed class PolicyConstantsTests
{
    [Test]
    public void ReplenishmentPolicy_ShouldKeepDistinctLookbackWindows()
    {
        Assert.That(ReplenishmentPolicy.ProjectionLookbackDays, Is.EqualTo(365));
        Assert.That(ReplenishmentPolicy.RecommendationLookbackDays, Is.EqualTo(90));
        Assert.That(ReplenishmentPolicy.ProjectionLookbackDays, Is.GreaterThan(ReplenishmentPolicy.RecommendationLookbackDays));
    }

    [Test]
    public void ForecastPolicy_ShouldKeepConfidenceBandsOrdered()
    {
        Assert.That(ForecastPolicy.HighConfidence, Is.GreaterThan(ForecastPolicy.MediumHighConfidence));
        Assert.That(ForecastPolicy.MediumHighConfidence, Is.GreaterThan(ForecastPolicy.MediumConfidence));
        Assert.That(ForecastPolicy.MediumConfidence, Is.GreaterThan(ForecastPolicy.LowConfidence));
        Assert.That(ForecastPolicy.LowConfidence, Is.GreaterThan(ForecastPolicy.NoHistoryConfidence));
    }

    [Test]
    public void ReviewActions_ShouldExposeWorkflowContractValues()
    {
        Assert.That(RecommendationReviewActions.Approve, Is.EqualTo("approve"));
        Assert.That(RecommendationReviewActions.Reject, Is.EqualTo("reject"));
        Assert.That(RecommendationReviewActions.Analysis, Is.EqualTo("analysis"));
    }
}
