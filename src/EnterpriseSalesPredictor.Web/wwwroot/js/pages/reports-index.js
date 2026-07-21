(function (namespace) {
    namespace.Pages.ReportsIndex = {
        init: function () {
            var reportsButton = document.getElementById("reports-export-button");
            var baseDataButton = document.getElementById("base-data-export-button");
            var statusSection = document.getElementById("reports-export-status");

            if (!reportsButton || !baseDataButton || !statusSection) {
                return;
            }

            reportsButton.addEventListener("click", async function () {
                await namespace.appUi.confirm("reports-export-modal", async function () {
                    await downloadWithState(reportsButton, "Preparando la exportación de reportes...", buildReportUrl());
                });
            });

            baseDataButton.addEventListener("click", async function () {
                await namespace.appUi.confirm("base-data-export-modal", async function () {
                    await downloadWithState(baseDataButton, "Preparando la exportación de datos base...", "/Exports/BaseData");
                });
            });

            async function downloadWithState(button, message, url) {
                button.disabled = true;
                namespace.Modules.statePanels.showAlert(statusSection, "info", "Exportando", message);

                try {
                    var result = await namespace.Utils.http.fetchBlob(url, {
                        headers: {
                            "X-Requested-With": "XMLHttpRequest"
                        }
                    });

                    namespace.Modules.downloads.saveBlob(result.blob, result.fileName || "export.xlsx");
                    namespace.Modules.statePanels.showAlert(statusSection, "success", "Completado", "La exportación se descargó correctamente.");
                    namespace.appUi.toast("La exportación se descargó correctamente.", "success");
                } catch (error) {
                    namespace.Modules.statePanels.showAlert(statusSection, "error", "Exportación fallida", error.message || "La exportación falló.");
                } finally {
                    button.disabled = false;
                }
            }

            function buildReportUrl() {
                var form = document.querySelector("form[method='get']");
                if (!form) {
                    return "/Exports/Reports";
                }

                var search = new URLSearchParams(new FormData(form));
                return "/Exports/Reports?" + search.toString();
            }
        }
    };

    namespace.Pages.ReportsIndex.init();
})(window.EnterpriseSalesPredictor);
