(function (namespace) {
    namespace.Utils.dom = {
        escapeHtml: function (value) {
            return String(value ?? "")
                .replace(/&/g, "&amp;")
                .replace(/</g, "&lt;")
                .replace(/>/g, "&gt;")
                .replace(/\"/g, "&quot;")
                .replace(/'/g, "&#39;");
        },
        formatDate: function (value) {
            if (!value) {
                return "";
            }

            var date = new Date(value);
            return Number.isNaN(date.getTime()) ? value : date.toISOString().slice(0, 10);
        },
        formatDateTime: function (value) {
            if (!value) {
                return "-";
            }

            var date = new Date(value);
            return Number.isNaN(date.getTime()) ? value : date.toISOString().replace("T", " ").slice(0, 16);
        }
    };
})(window.EnterpriseSalesPredictor);
