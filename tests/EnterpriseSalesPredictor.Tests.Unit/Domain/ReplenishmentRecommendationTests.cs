using EnterpriseSalesPredictor.Domain.Entities;

namespace EnterpriseSalesPredictor.Tests.Unit.Domain;

public sealed class ReplenishmentRecommendationTests
{
    [Test]
    public void Approve_ShouldUpdateReviewState()
    {
        var recommendation = CreateRecommendation();

        recommendation.Approve("manager", "approved");

        Assert.Multiple(() =>
        {
            Assert.That(recommendation.Status, Is.EqualTo(RecommendationStatus.Approved));
            Assert.That(recommendation.ReviewedBy, Is.EqualTo("manager"));
            Assert.That(recommendation.ReviewNotes, Is.EqualTo("approved"));
            Assert.That(recommendation.ReviewedAtUtc, Is.Not.Null);
        });
    }

    [Test]
    public void Reject_ShouldUpdateReviewState()
    {
        var recommendation = CreateRecommendation();

        recommendation.Reject("manager", "rejected");

        Assert.Multiple(() =>
        {
            Assert.That(recommendation.Status, Is.EqualTo(RecommendationStatus.Rejected));
            Assert.That(recommendation.ReviewedBy, Is.EqualTo("manager"));
        });
    }

    [Test]
    public void MarkForAnalysis_ShouldMoveToLowConfidenceState()
    {
        var recommendation = CreateRecommendation();

        recommendation.MarkForAnalysis("manager", "needs review");

        Assert.Multiple(() =>
        {
            Assert.That(recommendation.Status, Is.EqualTo(RecommendationStatus.LowConfidence));
            Assert.That(recommendation.ReviewedBy, Is.EqualTo("manager"));
            Assert.That(recommendation.ReviewNotes, Is.EqualTo("needs review"));
        });
    }

    private static ReplenishmentRecommendation CreateRecommendation()
    {
        return new ReplenishmentRecommendation(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 15m, 0.8m, "rationale");
    }
}
