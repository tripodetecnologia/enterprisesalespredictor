(function (namespace) {
    namespace.Pages.UploadsIndex = {
        init: function () {
            var form = document.getElementById("upload-form");
            var input = document.getElementById("upload-file-input");
            var dropzone = document.getElementById("upload-dropzone");
            var progress = document.getElementById("upload-progress");
            var progressBar = document.getElementById("upload-progress-bar");

            if (!form || !input || !dropzone || !progress || !progressBar) {
                return;
            }

            dropzone.addEventListener("dragover", function (event) {
                event.preventDefault();
                dropzone.classList.add("upload-dropzone-active");
            });

            dropzone.addEventListener("dragleave", function () {
                dropzone.classList.remove("upload-dropzone-active");
            });

            dropzone.addEventListener("drop", function (event) {
                event.preventDefault();
                dropzone.classList.remove("upload-dropzone-active");

                if (event.dataTransfer && event.dataTransfer.files && event.dataTransfer.files.length > 0) {
                    input.files = event.dataTransfer.files;
                }
            });

            form.addEventListener("submit", function () {
                progress.hidden = false;
                namespace.Modules.chartBars.setWidth(progressBar, 35);

                window.setTimeout(function () {
                    namespace.Modules.chartBars.setWidth(progressBar, 70);
                }, 250);
            });
        }
    };

    namespace.Pages.UploadsIndex.init();
})(window.EnterpriseSalesPredictor);
