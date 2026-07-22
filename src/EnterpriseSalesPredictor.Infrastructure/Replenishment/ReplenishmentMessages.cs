namespace EnterpriseSalesPredictor.Infrastructure.Replenishment;

public static class ReplenishmentMessages
{
    public const string ProductNotFound = "No encontramos el producto seleccionado.";

    public const string RecommendedUnitsMustBeGreaterThanZero = "La cantidad recomendada debe ser mayor que cero.";

    public const string RecommendationNotFound = "No encontramos la sugerencia solicitada.";

    public const string ReviewerRoleNotAllowed = "Tu rol no tiene permiso para aprobar o rechazar sugerencias de abastecimiento.";

    public const string InvalidReviewAction = "La acción de revisión debe ser aprobar, rechazar o enviar a análisis.";

    public const string InvalidDateRange = "Debes indicar un rango de fechas válido.";

    public const string ProjectionDatesMustBeFuture = "Las fechas de proyección deben ser futuras.";

    public static string ProjectionRangeOutOfBounds(int minimumDays, int maximumDays)
    {
        return $"El rango de proyección debe estar entre {minimumDays} y {maximumDays} días.";
    }
}
