(function (namespace) {
    namespace.Pages.ForecastsIndex = {
        init: function () {
            let form = document.getElementById("forecast-form");
            let loading = document.getElementById("forecast-loading");
            let error = document.getElementById("forecast-error");
            let result = document.getElementById("forecast-result");
            let customerSection = document.getElementById("forecast-customer-section");
            let productSection = document.getElementById("forecast-product-section");

            if (!form || !loading || !error || !result || !customerSection || !productSection) {
                return;
            }

            let projectedSales = document.getElementById("forecast-projected-sales");
            let confidence = document.getElementById("forecast-confidence");
            let generatedAt = document.getElementById("forecast-generated-at");
            let generatedBy = document.getElementById("forecast-generated-by");
            let summary = document.getElementById("forecast-summary");

            let customerBody = document.getElementById("forecast-customer-body");
            let customerPrev = document.getElementById("forecast-customer-prev");
            let customerNext = document.getElementById("forecast-customer-next");
            let customerIndicator = document.getElementById("forecast-customer-indicator");
            let customerPages = document.getElementById("forecast-customer-pages");
            let customerSummary = document.getElementById("forecast-customer-summary");

            let productBody = document.getElementById("forecast-product-body");
            let productPrev = document.getElementById("forecast-product-prev");
            let productNext = document.getElementById("forecast-product-next");
            let productIndicator = document.getElementById("forecast-product-indicator");
            let productPages = document.getElementById("forecast-product-pages");
            let productSummary = document.getElementById("forecast-product-summary");

            let customerState = { items: [], page: namespace.Constants.pagination.firstPage, pageSize: namespace.Constants.pagination.forecastPageSize };
            let productState = { items: [], page: namespace.Constants.pagination.firstPage, pageSize: namespace.Constants.pagination.forecastPageSize };

            form.addEventListener("submit", async function (event) {
                event.preventDefault();
                let request = buildPayload(form);

                if (!request.fromDate) {
                    namespace.Modules.statePanels.showAlert(error, namespace.Constants.uiVariants.error, "Dato requerido", "La fecha de inicio es obligatoria.");
                    hideResults();
                    return;
                }

                if (!request.toDate) {
                    namespace.Modules.statePanels.showAlert(error, namespace.Constants.uiVariants.error, "Dato requerido", "La fecha de fin es obligatoria.");
                    hideResults();
                    return;
                }

                if (!request.productName) {
                    namespace.Modules.statePanels.showAlert(error, namespace.Constants.uiVariants.error, "Dato requerido", "El producto es obligatorio.");
                    hideResults();
                    return;
                }

                loading.hidden = false;
                namespace.Modules.statePanels.clear(error);
                hideResults();

                try {
                    let payload = await namespace.Utils.http.fetchJson("/Forecasts/Generate", {
                        method: "POST",
                        headers: {
                            "Content-Type": namespace.Constants.http.jsonContentType
                        },
                        body: JSON.stringify(request)
                    });

                    renderSummary(payload);

                    customerState.items = payload.customerMonthlyForecasts || [];
                    customerState.page = namespace.Constants.pagination.firstPage;
                    renderCustomerTable();

                    productState.items = payload.productMonthlyForecasts || [];
                    productState.page = namespace.Constants.pagination.firstPage;
                    renderProductTable();

                    result.hidden = false;
                    customerSection.hidden = false;
                    productSection.hidden = false;
                } catch (requestError) {
                    namespace.Modules.statePanels.showAlert(error, namespace.Constants.uiVariants.error, "Proyección fallida", requestError.message || "Se produjo un error inesperado al generar la proyección.");
                } finally {
                    loading.hidden = true;
                }
            });

            customerPrev.addEventListener("click", function () {
                if (customerState.page > namespace.Constants.pagination.firstPage) {
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
                if (productState.page > namespace.Constants.pagination.firstPage) {
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
                projectedSales.textContent = namespace.Utils.number.formatInteger(payload.projectedSales);
                confidence.textContent = Math.round(Number(payload.confidence || 0) * 100) + "%";
                generatedAt.textContent = namespace.Utils.dom.formatDateTime(payload.generatedAtUtc);
                generatedBy.textContent = payload.generatedBy || "-";
                summary.textContent = payload.explanation || "No hay explicación disponible.";
            }

            function renderCustomerTable() {
                let items = slice(customerState);                
                namespace.Modules.tableRenderer.renderRows(customerBody, items, renderCustomerRow, 4);
                customerSummary.textContent = buildSummary(customerState);
                renderPager(customerState, customerIndicator, customerPages, customerPrev, customerNext, renderCustomerTable);
            }

            function renderProductTable() {
                let items = slice(productState);
                namespace.Modules.tableRenderer.renderRows(productBody, items, renderProductRow, 5);
                productSummary.textContent = buildSummary(productState);
                renderPager(productState, productIndicator, productPages, productPrev, productNext, renderProductTable);
            }

            function renderPager(state, indicator, pagesContainer, prevButton, nextButton, rerender) {
                let pages = totalPages(state);
                indicator.textContent = "Página " + state.page + " de " + Math.max(pages, 1);
                prevButton.disabled = state.page <= namespace.Constants.pagination.firstPage;
                nextButton.disabled = state.page >= pages;
                pagesContainer.innerHTML = "";

                if (pages <= namespace.Constants.pagination.firstPage) {
                    return;
                }

                let start = Math.max(namespace.Constants.pagination.firstPage, state.page - namespace.Constants.pagination.pageLinkRadius);
                let end = Math.min(pages, state.page + namespace.Constants.pagination.pageLinkRadius);

                for (let page = start; page <= end; page++) {
                    let button = document.createElement("button");
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

                let start = ((state.page - namespace.Constants.pagination.firstPage) * state.pageSize) + namespace.Constants.pagination.firstPage;
                let end = Math.min(start + state.pageSize - namespace.Constants.pagination.firstPage, state.items.length);
                return "Mostrando " + start + " a " + end + " de " + state.items.length + " registros totales.";
            }

            function slice(state) {
                let start = (state.page - namespace.Constants.pagination.firstPage) * state.pageSize;
                return state.items.slice(start, start + state.pageSize);
            }

            function totalPages(state) {
                return state.items.length === 0 ? namespace.Constants.pagination.firstPage : Math.ceil(state.items.length / state.pageSize);
            }

            function renderCustomerRow(row) {
                return "<td>" + namespace.Utils.dom.escapeHtml(row.monthLabel) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.customerName) + "</td>"
                    + "<td>" + namespace.Utils.number.formatInteger(row.projectedSales) + "</td>"
                    + "<td>" + Math.round(Number(row.confidence || 0) * 100) + "%</td>";
            }

            function renderProductRow(row) {
                return "<td>" + namespace.Utils.dom.escapeHtml(row.monthLabel) + "</td>"
                    + "<td>" + namespace.Utils.dom.escapeHtml(row.productName) + "</td>"
                    + "<td>" + namespace.Utils.number.formatInteger(row.projectedUnits) + "</td>"
                    + "<td>" + namespace.Utils.number.formatInteger(row.projectedSales) + "</td>"
                    + "<td>" + Math.round(Number(row.confidence || 0) * 100) + "%</td>";
            }

            function buildPayload(sourceForm) {
                let formData = new FormData(sourceForm);
                return {
                    fromDate: formData.get("Filters.FromDate") || null,
                    toDate: formData.get("Filters.ToDate") || null,
                    customerId: formData.get("Filters.CustomerId") || null,
                    productName: formData.get("Filters.ProductName") || null
                };
            }
        }
    };

    namespace.Pages.ForecastsIndex.init();
})(window.EnterpriseSalesPredictor);
