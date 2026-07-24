(function (namespace) {
    namespace.Pages.ReplenishmentApprovalsIndex = {
        init: function () {
            var page = document.getElementById("replenishment-approvals-page");
            if (!page) {
                return;
            }

            if (page.dataset.statusMessage) {
                namespace.Modules.appUi.toast(page.dataset.statusMessage, namespace.Constants.uiVariants.success);
            }

            if (page.dataset.errorMessage) {
                namespace.Modules.appUi.toast(page.dataset.errorMessage, namespace.Constants.uiVariants.error);
            }

            document.querySelectorAll(".approval-form").forEach(function (form) {
                form.addEventListener("submit", function (event) {
                    event.preventDefault();
                    var modalId = form.getAttribute("data-modal-id");
                    namespace.Modules.appUi.confirm(modalId, async function () {
                        form.submit();
                    });
                });
            });
        }
    };

    namespace.Pages.ReplenishmentApprovalsIndex.init();
})(window.EnterpriseSalesPredictor);
