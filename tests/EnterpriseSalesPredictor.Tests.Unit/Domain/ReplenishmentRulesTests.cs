using EnterpriseSalesPredictor.Domain.Rules;

namespace EnterpriseSalesPredictor.Tests.Unit.Domain;

public sealed class ReplenishmentRulesTests
{
    [Test]
    public void CanApprove_ShouldAllowConfiguredRoles()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ReplenishmentRules.CanApprove("PurchaseManager"), Is.True);
            Assert.That(ReplenishmentRules.CanApprove("WarehouseManager"), Is.True);
        });
    }

    [Test]
    public void CanApprove_ShouldRejectUnknownRole()
    {
        Assert.That(ReplenishmentRules.CanApprove("SalesManager"), Is.False);
    }

    [Test]
    public void ShouldGenerateRecommendation_ShouldCompareDemandAgainstStock()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ReplenishmentRules.ShouldGenerateRecommendation(12m, 5), Is.True);
            Assert.That(ReplenishmentRules.ShouldGenerateRecommendation(5m, 5), Is.False);
            Assert.That(ReplenishmentRules.ShouldGenerateRecommendation(4m, 10), Is.False);
        });
    }
}
