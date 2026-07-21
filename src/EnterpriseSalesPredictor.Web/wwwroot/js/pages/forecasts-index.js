(function (namespace) {
    namespace.Pages.ForecastsIndex = {
        init: function () {
            var form = document.getElementById("forecast-form");
            var loading = document.getElementById("forecast-loading");
            var error = document.getElementById("forecast-error");
            var result = document.getElementById("forecast-result");
            var customerSection = document.getElementById("forecast-customer-section");
            var productSection = document.getElementById("forecast-product-section");

            if (!form || !loading || !error || !result || !customerSection || !productSection) {
                return;
            }

            var projectedSales = document.getElementById("forecast-projected-sales");
            var confidence = document.getElementById("forecast-confidence");
            var generatedAt = document.getElementById("forecast-generated-at");
            var generatedBy = document.getElementById("forecast-generated-by");
            var summary = document.getElementById("forecast-summary");

            var customerBody = document.getElementById("forecast-customer-body");
            var customerPrev = document.getElementById("forecast-customer-prev");
            var customerNext = document.getElementById("forecast-customer-next");
            var customerIndicator = document.getElementById("forecast-customer-indicator");
            var customerPages = document.getElementById("forecast-customer-pages");
            var customerSummary = document.getElementById("forecast-customer-summary");

            var productBody = document.getElementById("forecast-product-body");
            var productPrev = document.getElementById("forecast-product-prev");
            var productNext = document.getElementById("forecast-product-next");
            var productIndicator = document.getElementById("forecast-product-indicator");
            var productPages = document.getElementById("forecast-product-pages");
            var productSummary = document.getElementById("forecast-product-summary");

            var customerState = { items: [], page: 1, pageSize: 10 };
            var productState = { items: [], page: 1, pageSize: 10 };

            form.addEventListener("submit", async function (event) {
                event.preventDefault();
                var request = buildPayload(form);

                if (!request.fromDate) {
                    namespace.Modules.statePanels.showAlert(error, "error", "Dato requerido", "La fecha de inicio es obligatoria.");
                    hideResults();
                    return;
                }

                if (!request.toDate) {
                    namespace.Modules.statePanels.showAlert(error, "error", "Dato requerido", "La fecha de fin es obligatoria.");
                    hideResults();
                    return;
                }

                loading.hidden = false;
                namespace.Modules.statePanels.clear(error);
                hideResults();

                try {
                    var payload = await namespace.Utils.http.fetchJson("/Forecasts/Generate", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            "X-Requested-With": "XMLHttpRequest"
                        },
                        body: JSON.stringify(request)
                    });

                    renderSummary(payload);

                    customerState.items = payload.customerMonthlyForecasts || [];
                    customerState.page = 1;
                    renderCustomerTable();

                    productState.items = payload.productMonthlyForecasts || [];
                    productState.page = 1;
                    renderProductTable();

                    result.hidden = false;
                    customerSection.hidden = false;
                    productSection.hidden = false;
                } catch (requestError) {
                    namespace.Modules.statePanels.showAlert(error, "error", "Proyección fallida", requestError.message || "Se produjo un error inesperado al generar la proyección.");
                } finally {
                    loading.hidden = true;
                }
            });

            customerPrev.addEventListener("click", function () {
                if (customerState.page > 1) {
                    customerState.page--;
                    renderCustomerTable();
                }
            });

            customerNext.addEventListener("click", function () {
                if (customerState.page < totalPages(customerState)) {
                    customerState.page++;
                    renderCustomerTable();
                }
            });

            productPrev.addEventListener("click", function () {
                if (productState.page > 1) {
                    productState.page--;
                    renderProductTable();
                }
            });

            productNext.addEventListener("click", function () {
                if (productState.page < totalPages(productState)) {
                    productState.page++;
                    renderProductTable();
                }
            });

            function hideResults() {
                result.hidden = true;
                customerSection.hidden = true;
                productSection.hidden = true;
            }

            function renderSummary(payload) {
                projectedSales.textContent = Number(payload.projectedSales || 0).toFixed(2);
                confidence.textContent = Math.round(Number(payload.confidence || 0) * 100) + "%";
                generatedAt.textContent = namespace.Utils.dom.formatDateTime(payload.generatedAtUtc);
                generatedBy.textContent = payload.generatedBy || "-";
                summary.textContent = payload.explanation || "No hay explicación disponible.";
            }

            function renderCustomerTable() {
                var items = slice(customerState);
                namespace.Modules.tableRenderer.renderRows(customerBody, items, renderCustomerRow, 4);
                customerSummary.textContent = buildSummary(customerState);
                renderPager(customerState, customerIndicator, customerPages, customerPrev, customerNext, renderCustomerTable);
            }

            function renderProductTable() {
                var items = slice(productState);
                namespace.Modules.tableRenderer.renderRows(productBody, items, renderProductRow, 5);
                productSummary.textContent = buildSummary(productState);
                renderPager(productState, productIndicator, productPages, productPrev, productNext, renderProductTable);
            }

            function renderPager(state, indicator, pagesContainer, prevButton, nextButton, rerender) {
                var pages = totalPages(state);
                indicator.textContent = "Página " + state.page + " de " + Math.max(pages, 1);
                prevButton.disabled = state.page <= 1;
                nextButton.disabled = state.page >= pages;
                pagesContainer.innerHTML = "";

                if (pages <= 1) {
                    return;
                }

                var start = Math.max(1, state.page - 2);
                var end = Math.min(pages, state.page + 2);

                for (var page = start; page <= end; page++) {
                    var button = document.createElement("button");
                    button.type = "button";
                    button.className = page === state.page ? "sales-page-link sales-page-link-active" : "sales-page-link";
                    button.textContent = String(page);
                    button.disabled = page === state.page;
                    button.addEventListener("click", function (selectedPage) {
                        return function () {
                            state.page = selectedPage;
                            rerender();
                        };
                    }(page));
                    pagesContainer.appendChild(button);
                }
            }

            function buildSummary(state) {
                if (!state.items.length) {
                    return "Mostrando 0 registros.";
                }

                var start = ((state.page - 1) * state.pageSize) + 1;
                var end = Math.min(start + state.pageSize - 1, state.items.length);
                return "Mostrando " + start + " a " + end + " de " + state.items.length + " registros totales.";
            }

            function slice(state) {
                var start = (state.page - 1) * state.pageSize;
                return state.items.slice(start, start + state.pageSize);
            }

            function totalPages(state) {
                return state.items.length === 0 ? 1 : Math.ceil(state.items.length / state.pageSize);
            }

            function renderCustomerRow(row) {
                return "<td>" + namespace.Utils.dom.escapeHtml(row.monthLabel) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.customerName) + "</td>"
                    + "<td>" + Number(row.projectedSales || 0).toFixed(2) + "</td>"
                    + "<td>" + Math.round(Number(row.confidence || 0) * 100) + "%</td>";
            }

            function renderProductRow(row) {
                return "<td>" + namespace.Utils.dom.escapeHtml(row.monthLabel) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.productName) + "</td>"
                    + "<td>" + Number(row.projectedUnits || 0).toFixed(2) + "</td>"
                    + "<td>" + Number(row.projectedSales || 0).toFixed(2) + "</td>"
                    + "<td>" + Math.round(Number(row.confidence || 0) * 100) + "%</td>";
            }

            function buildPayload(sourceForm) {
                var formData = new FormData(sourceForm);
                return {
                    fromDate: formData.get("Filters.FromDate") || null,
                    toDate: formData.get("Filters.ToDate") || null,
                    customerId: formData.get("Filters.CustomerId") || null,
                    productId: formData.get("Filters.ProductId") || null
                };
            }
        }
    };

    namespace.Pages.ForecastsIndex.init();
})(window.EnterpriseSalesPredictor);
