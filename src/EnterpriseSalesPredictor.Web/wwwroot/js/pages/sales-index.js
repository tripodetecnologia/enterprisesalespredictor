(function (namespace) {
    namespace.Pages.SalesIndex = {
        init: function () {
            var form = document.getElementById("sales-query-form");
            var loading = document.getElementById("sales-loading");
            var error = document.getElementById("sales-error");
            var empty = document.getElementById("sales-empty");
            var body = document.getElementById("sales-results-body");
            var exportButton = document.getElementById("sales-export-button");
            var exportStatus = document.getElementById("sales-export-status");

            if (!form || !loading || !error || !empty || !body || !exportButton || !exportStatus) {
                return;
            }

            form.addEventListener("submit", async function (event) {
                event.preventDefault();
                loading.hidden = false;
                namespace.Modules.statePanels.clear(error);
                empty.hidden = true;

                try {
                    var payload = await namespace.Utils.http.fetchJson("/Sales/Query?" + buildSearchParams(form).toString(), {
                        headers: {
                            "X-Requested-With": "XMLHttpRequest"
                        }
                    });

                    namespace.Modules.tableRenderer.renderRows(body, payload, renderRowHtml, 9);
                    empty.hidden = payload.length !== 0;
                } catch (fetchError) {
                    body.innerHTML = "";
                    namespace.Modules.statePanels.showAlert(error, "error", "Query failed", fetchError.message || "Unexpected error loading sales results.");
                } finally {
                    loading.hidden = true;
                }
            });

            exportButton.addEventListener("click", async function () {
                await namespace.appUi.confirm("sales-export-modal", async function () {
                    exportButton.disabled = true;
                    namespace.Modules.statePanels.showAlert(exportStatus, "info", "Exporting", "Preparing sales export...");
                    namespace.Modules.statePanels.clear(error);

                    try {
                        var result = await namespace.Utils.http.fetchBlob("/Exports/FilteredSales?" + buildSearchParams(form).toString(), {
                            headers: {
                                "X-Requested-With": "XMLHttpRequest"
                            }
                        });
                        namespace.Modules.downloads.saveBlob(result.blob, result.fileName || "sales-export.xlsx");
                        namespace.Modules.statePanels.showAlert(exportStatus, "success", "Completed", "Sales export downloaded successfully.");
                        namespace.appUi.toast("Sales export downloaded.", "success");
                    } catch (downloadError) {
                        namespace.Modules.statePanels.showAlert(error, "error", "Export failed", downloadError.message || "Unexpected error exporting sales data.");
                        namespace.Modules.statePanels.showAlert(exportStatus, "error", "Export failed", "Sales export failed.");
                    } finally {
                        exportButton.disabled = false;
                    }
                });
            });

            function buildSearchParams(sourceForm) {
                var formData = new FormData(sourceForm);
                var search = new URLSearchParams();

                for (var entry of formData.entries()) {
                    if (entry[1]) {
                        search.append(entry[0], entry[1].toString());
                    }
                }

                return search;
            }

            function renderRowHtml(row) {
                return "<td>" + namespace.Utils.dom.formatDate(row.saleDate) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.invoiceNumber) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.customerId) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.productId) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.supplierId) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.sellerId) + "</td>"
                    + "<td>" + row.quantity + "</td>"
                    + "<td>" + row.saleAmount + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.paymentMethod) + "</td>";
            }
        }
    };

    namespace.Pages.SalesIndex.init();
})(window.EnterpriseSalesPredictor);
