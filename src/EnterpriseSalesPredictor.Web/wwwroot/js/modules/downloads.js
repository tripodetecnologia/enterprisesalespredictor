(function (namespace) {
    namespace.Modules.downloads = {
        saveBlob: function (blob, fileName) {
            var objectUrl = window.URL.createObjectURL(blob);
            var anchor = document.createElement("a");
            anchor.href = objectUrl;
            anchor.download = fileName || "download.bin";
            document.body.appendChild(anchor);
            anchor.click();
            anchor.remove();
            window.URL.revokeObjectURL(objectUrl);
        }
    };
})(window.EnterpriseSalesPredictor);
