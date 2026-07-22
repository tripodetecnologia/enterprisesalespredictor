(function (namespace) {
    namespace.Utils.number = {
        formatNumber: function (value, options) {
            var numericValue = Number(value || 0);
            var settings = options || {};

            return numericValue.toLocaleString(settings.locale || "es-CO", {
                minimumFractionDigits: settings.minimumFractionDigits ?? 0,
                maximumFractionDigits: settings.maximumFractionDigits ?? 0
            });
        },
        formatInteger: function (value, locale) {
            return this.formatNumber(value, {
                locale: locale || "es-CO",
                minimumFractionDigits: 0,
                maximumFractionDigits: 0
            });
        }
    };
})(window.EnterpriseSalesPredictor);
