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
            var prevButton = document.getElementById("sales-prev-page");
            var nextButton = document.getElementById("sales-next-page");
            var pageIndicator = document.getElementById("sales-page-indicator");
            var pageNumbers = document.getElementById("sales-page-numbers");
            var resultsSummary = document.getElementById("sales-results-summary");
            var pageNumberInput = form.querySelector("input[name='Filters.PageNumber']");

            if (!form || !loading || !error || !empty || !body || !exportButton || !exportStatus || !prevButton || !nextButton || !pageIndicator || !pageNumbers || !resultsSummary || !pageNumberInput) {
                return;
            }

            form.addEventListener("submit", async function (event) {
                event.preventDefault();
                pageNumberInput.value = "1";
                await loadPage();
            });

            prevButton.addEventListener("click", async function () {
                var currentPage = parseInt(pageNumberInput.value || "1", 10);
                if (currentPage <= 1) {
                    return;
                }

                pageNumberInput.value = String(currentPage - 1);
                await loadPage();
            });

            nextButton.addEventListener("click", async function () {
                var currentPage = parseInt(pageNumberInput.value || "1", 10);
                pageNumberInput.value = String(currentPage + 1);
                await loadPage();
            });

            exportButton.addEventListener("click", async function () {
                await namespace.appUi.confirm("sales-export-modal", async function () {
                    exportButton.disabled = true;
                    namespace.Modules.statePanels.showAlert(exportStatus, "info", "Exportando", "Preparando la exportación de ventas...");
                    namespace.Modules.statePanels.clear(error);

                    try {
                        var result = await namespace.Utils.http.fetchBlob("/Exports/FilteredSales?" + buildSearchParams(form).toString(), {
                            headers: {
                                "X-Requested-With": "XMLHttpRequest"
                            }
                        });
                        namespace.Modules.downloads.saveBlob(result.blob, result.fileName || "sales-export.xlsx");
                        namespace.Modules.statePanels.showAlert(exportStatus, "success", "Completado", "La exportación de ventas se descargó correctamente.");
                        namespace.appUi.toast("La exportación de ventas se descargó correctamente.", "success");
                    } catch (downloadError) {
                        namespace.Modules.statePanels.showAlert(error, "error", "Exportación fallida", downloadError.message || "Se produjo un error inesperado al exportar las ventas.");
                        namespace.Modules.statePanels.showAlert(exportStatus, "error", "Exportación fallida", "La exportación de ventas falló.");
                    } finally {
                        exportButton.disabled = false;
                    }
                });
            });

            async function loadPage() {
                loading.hidden = false;
                namespace.Modules.statePanels.clear(error);
                empty.hidden = true;
                prevButton.disabled = true;
                nextButton.disabled = true;

                try {
                    var payload = await namespace.Utils.http.fetchJson("/Sales/Query?" + buildSearchParams(form).toString(), {
                        headers: {
                            "X-Requested-With": "XMLHttpRequest"
                        }
                    });

                    namespace.Modules.tableRenderer.renderRows(body, payload.items, renderRowHtml, 9);
                    empty.hidden = payload.items.length !== 0;

                    pageIndicator.textContent = "Página " + Math.max(payload.pageNumber, 1) + " de " + Math.max(payload.totalPages, 1);
                    resultsSummary.textContent = buildSummary(payload);
                    prevButton.disabled = payload.pageNumber <= 1;
                    nextButton.disabled = payload.pageNumber >= payload.totalPages;
                    renderPageNumbers(payload);
                } catch (fetchError) {
                    body.innerHTML = "";
                    pageNumbers.innerHTML = "";
                    resultsSummary.textContent = "Mostrando 0 registros.";
                    namespace.Modules.statePanels.showAlert(error, "error", "Consulta fallida", fetchError.message || "Se produjo un error inesperado al cargar los resultados de ventas.");
                } finally {
                    loading.hidden = true;
                }
            }

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

            function buildSummary(payload) {
                if (!payload.totalCount || payload.items.length === 0) {
                    return "Mostrando 0 registros.";
                }

                var start = ((payload.pageNumber - 1) * payload.pageSize) + 1;
                var end = start + payload.items.length - 1;
                return "Mostrando " + start + " a " + end + " de " + payload.totalCount + " registros totales.";
            }

            function renderPageNumbers(payload) {
                pageNumbers.innerHTML = "";

                if (!payload.totalPages || payload.totalPages <= 1) {
                    return;
                }

                var start = Math.max(1, payload.pageNumber - 2);
                var end = Math.min(payload.totalPages, payload.pageNumber + 2);

                for (var page = start; page <= end; page++) {
                    var button = document.createElement("button");
                    button.type = "button";
                    button.className = page === payload.pageNumber ? "sales-page-link sales-page-link-active" : "sales-page-link";
                    button.textContent = String(page);
                    button.disabled = page === payload.pageNumber;
                    button.dataset.page = String(page);
                    button.addEventListener("click", async function (event) {
                        var target = event.currentTarget;
                        pageNumberInput.value = target.dataset.page || "1";
                        await loadPage();
                    });
                    pageNumbers.appendChild(button);
                }
            }

            function renderRowHtml(row) {
                return "<td>" + namespace.Utils.dom.formatDate(row.saleDate) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.invoiceNumber) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.customerName || row.customerId) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.productName || row.productId) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.supplierName || row.supplierId) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.sellerName || row.sellerId) + "</td>"
                    + "<td>" + row.quantity + "</td>"
                    + "<td>" + row.saleAmount + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.paymentMethod) + "</td>";
            }
        }
    };

    namespace.Pages.SalesIndex.init();
})(window.EnterpriseSalesPredictor);
