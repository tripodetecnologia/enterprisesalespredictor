(function (namespace) {
    namespace.Modules.chartBars = {
        setWidth: function (element, percent) {
            if (!element) {
                return;
            }

            var safePercent = Math.max(0, Math.min(100, percent));
            element.style.width = safePercent + "%";
        }
    };
})(window.EnterpriseSalesPredictor);
