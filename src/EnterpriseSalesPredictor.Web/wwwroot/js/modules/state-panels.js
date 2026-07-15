(function (namespace) {
    namespace.Modules.statePanels = {
        showAlert: function (element, variant, title, message) {
            if (!element) {
                return;
            }

            element.hidden = false;
            element.innerHTML = '<div class="app-alert app-alert-' + variant + '"><strong>'
                + namespace.Utils.dom.escapeHtml(title)
                + '</strong><span>'
                + namespace.Utils.dom.escapeHtml(message)
                + '</span></div>';
        },
        clear: function (element) {
            if (!element) {
                return;
            }

            element.hidden = true;
            element.innerHTML = "";
        }
    };
})(window.EnterpriseSalesPredictor);
