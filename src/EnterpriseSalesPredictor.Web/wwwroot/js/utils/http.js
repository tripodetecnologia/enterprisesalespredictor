(function (namespace) {
    namespace.Utils.http = {
        fetchJson: async function (url, options) {
            var response = await fetch(url, options);
            if (!response.ok) {
                throw new Error(await response.text() || "Request failed.");
            }

            return await response.json();
        },
        fetchBlob: async function (url, options) {
            var response = await fetch(url, options);
            if (!response.ok) {
                throw new Error(await response.text() || "Request failed.");
            }

            return {
                blob: await response.blob(),
                fileName: getFileName(response)
            };
        }
    };

    function getFileName(response) {
        var disposition = response.headers.get("content-disposition");
        if (!disposition) {
            return null;
        }

        var match = disposition.match(/filename\*?=(?:UTF-8'')?"?([^";]+)"?/i);
        return match ? decodeURIComponent(match[1]) : null;
    }
})(window.EnterpriseSalesPredictor);
