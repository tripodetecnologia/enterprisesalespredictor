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
            return Number.isNaN(date.getTime()) ? value : date.toISOString().split("T")[0];
        },
        formatDateTime: function (value) {
            if (!value) {
                return "-";
            }

            var date = new Date(value);
            return Number.isNaN(date.getTime()) ? value : formatDateTimeParts(date);
        }
    };

    function formatDateTimeParts(date) {
        var parts = date.toISOString().split("T");
        var time = parts[1].split(":");
        return parts[0] + " " + time[0] + ":" + time[1];
    }
})(window.EnterpriseSalesPredictor);
