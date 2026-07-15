namespace EnterpriseSalesPredictor.Web.ViewModels.Dashboard;

public sealed class DashboardPageViewModel
{
    public DashboardKpiViewModel Kpis { get; set; } = new();

    public IReadOnlyCollection<DashboardBreakdownItemViewModel> TopCustomers { get; set; } = Array.Empty<DashboardBreakdownItemViewModel>();

    public IReadOnlyCollection<DashboardBreakdownItemViewModel> TopProducts { get; set; } = Array.Empty<DashboardBreakdownItemViewModel>();

    public IReadOnlyCollection<DashboardBreakdownItemViewModel> SalesByLine { get; set; } = Array.Empty<DashboardBreakdownItemViewModel>();

    public IReadOnlyCollection<DashboardBreakdownItemViewModel> SalesBySupplier { get; set; } = Array.Empty<DashboardBreakdownItemViewModel>();

    public IReadOnlyCollection<DashboardAlertViewModel> Alerts { get; set; } = Array.Empty<DashboardAlertViewModel>();
}
