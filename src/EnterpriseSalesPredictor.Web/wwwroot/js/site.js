(function () {
    window.EnterpriseSalesPredictor = window.EnterpriseSalesPredictor || {};
    window.EnterpriseSalesPredictor.Utils = window.EnterpriseSalesPredictor.Utils || {};
    window.EnterpriseSalesPredictor.Modules = window.EnterpriseSalesPredictor.Modules || {};
    window.EnterpriseSalesPredictor.Pages = window.EnterpriseSalesPredictor.Pages || {};

    function ensureToastStack() {
        var stack = document.getElementById("app-toast-stack");
        if (!stack) {
            stack = document.createElement("div");
            stack.id = "app-toast-stack";
            stack.className = "app-toast-stack";
            document.body.appendChild(stack);
        }

        return stack;
    }

    function toast(message, variant) {
        var stack = ensureToastStack();
        var item = document.createElement("div");
        item.className = "app-toast app-toast-" + (variant || "success");
        item.textContent = message;
        stack.appendChild(item);

        window.setTimeout(function () {
            item.remove();
        }, 3200);
    }

    async function confirm(modalId, onConfirm) {
        var modal = document.getElementById(modalId);
        if (!modal || typeof modal.showModal !== "function") {
            if (window.confirm("Confirmá esta acción.")) {
                await onConfirm();
            }

            return;
        }

        modal.showModal();

        var cancelButton = modal.querySelector("[data-modal-cancel='" + modalId + "']");
        var confirmButton = modal.querySelector("[data-modal-confirm='" + modalId + "']");

        var cleanup = function () {
            if (cancelButton) {
                cancelButton.onclick = null;
            }

            if (confirmButton) {
                confirmButton.onclick = null;
            }
        };

        if (cancelButton) {
            cancelButton.onclick = function () {
                cleanup();
                modal.close();
            };
        }

        if (confirmButton) {
            confirmButton.onclick = async function () {
                cleanup();
                modal.close();
                await onConfirm();
            };
        }
    }

    window.appUi = {
        confirm: confirm,
        toast: toast
    };

    window.EnterpriseSalesPredictor.appUi = window.appUi;
})();
