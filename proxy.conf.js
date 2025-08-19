const PROXY_CONFIG = {
    "/api/*": {
        target: "https://localhost:7136",
        secure: false,
        changeOrigin: true,
        logLevel: "debug",
        onProxyReq: function (proxyReq, req, res) {
            console.log('Proxying request to:', proxyReq.path);
        },
        onProxyRes: function (proxyRes, req, res) {
            console.log('Received response:', proxyRes.statusCode);
        },
        onError: function (err, req, res) {
            console.log('Proxy error:', err);
        }
    }
};

module.exports = PROXY_CONFIG;
