(function (namespace) {
    namespace.Modules.tableRenderer = {
        renderRows: function (container, rows, renderRowHtml, emptyColSpan) {
            if (!container) {
                return;
            }

            container.innerHTML = "";

            if (!rows || rows.length === 0) {
                container.innerHTML = '<tr><td colspan="' + emptyColSpan + '">No results.</td></tr>';
                return;
            }

            rows.forEach(function (row) {
                var tr = document.createElement("tr");
                tr.innerHTML = renderRowHtml(row);
                container.appendChild(tr);
            });
        }
    };
})(window.EnterpriseSalesPredictor);
