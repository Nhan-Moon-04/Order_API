const PROXY_CONFIG = {
  "/api/**": {
    target: "https://localhost:7136",
    secure: false,
    changeOrigin: true,
    logLevel: "debug"
  }
};

module.exports = PROXY_CONFIG;
