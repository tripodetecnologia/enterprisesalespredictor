(function (namespace) {
    namespace.Utils.http = {
        fetchJson: async function (url, options) {
            var response = await fetch(url, withAjaxHeaders(options));
            if (!response.ok) {
                throw new Error(await response.text() || "Request failed.");
            }

            return await response.json();
        },
        fetchBlob: async function (url, options) {
            var response = await fetch(url, withAjaxHeaders(options));
            if (!response.ok) {
                throw new Error(await response.text() || "Request failed.");
            }

            return {
                blob: await response.blob(),
                fileName: getFileName(response)
            };
        }
    };

    function withAjaxHeaders(options) {
        var settings = options || {};
        var headers = new Headers(settings.headers || {});
        var http = namespace.Constants.http;
        if (!headers.has(http.ajaxHeaderName)) {
            headers.set(http.ajaxHeaderName, http.ajaxHeaderValue);
        }

        return Object.assign({}, settings, { headers: headers });
    }

    function getFileName(response) {
        var disposition = response.headers.get("content-disposition");
        if (!disposition) {
            return null;
        }

        var match = disposition.match(/filename\*?=(?:UTF-8'')?"?([^";]+)"?/i);
        return match ? decodeURIComponent(match[1]) : null;
    }
})(window.EnterpriseSalesPredictor);
