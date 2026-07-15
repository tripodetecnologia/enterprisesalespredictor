(function (namespace) {
    namespace.Pages.ForecastsIndex = {
        init: function () {
            var form = document.getElementById("forecast-form");
            var loading = document.getElementById("forecast-loading");
            var error = document.getElementById("forecast-error");
            var result = document.getElementById("forecast-result");

            if (!form || !loading || !error || !result) {
                return;
            }

            var projectedSales = document.getElementById("forecast-projected-sales");
            var confidence = document.getElementById("forecast-confidence");
            var generatedAt = document.getElementById("forecast-generated-at");
            var generatedBy = document.getElementById("forecast-generated-by");
            var summary = document.getElementById("forecast-summary");
            var chartValue = document.getElementById("forecast-chart-value");
            var chartFill = document.getElementById("forecast-chart-fill");

            form.addEventListener("submit", async function (event) {
                event.preventDefault();
                loading.hidden = false;
                namespace.Modules.statePanels.clear(error);
                result.hidden = true;

                try {
                    var payload = await namespace.Utils.http.fetchJson("/Forecasts/Generate", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            "X-Requested-With": "XMLHttpRequest"
                        },
                        body: JSON.stringify(buildPayload(form))
                    });

                    render(payload);
                } catch (requestError) {
                    namespace.Modules.statePanels.showAlert(error, "error", "Forecast failed", requestError.message || "Unexpected forecasting error.");
                } finally {
                    loading.hidden = true;
                }
            });

            function render(payload) {
                projectedSales.textContent = Number(payload.projectedSales || 0).toFixed(2);
                confidence.textContent = Math.round(Number(payload.confidence || 0) * 100) + "%";
                generatedAt.textContent = namespace.Utils.dom.formatDateTime(payload.generatedAtUtc);
                generatedBy.textContent = payload.generatedBy || "-";
                summary.textContent = payload.explanation || "No explanation available.";
                chartValue.textContent = Number(payload.projectedSales || 0).toFixed(2);

                var confidencePercent = Math.max(5, Math.min(100, Math.round(Number(payload.confidence || 0) * 100)));
                namespace.Modules.chartBars.setWidth(chartFill, confidencePercent);
                result.hidden = false;
            }

            function buildPayload(sourceForm) {
                var formData = new FormData(sourceForm);
                return {
                    fromDate: formData.get("Filters.FromDate") || null,
                    toDate: formData.get("Filters.ToDate") || null,
                    productId: formData.get("Filters.ProductId") || null,
                    customerId: formData.get("Filters.CustomerId") || null
                };
            }
        }
    };

    namespace.Pages.ForecastsIndex.init();
})(window.EnterpriseSalesPredictor);
