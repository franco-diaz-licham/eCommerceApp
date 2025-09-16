// import { defineConfig } from "vite";
import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react-swc";
import mkcert from "vite-plugin-mkcert";

// https://vite.dev/config/
export default defineConfig({
    server: {
        port: 3000,
    },
    plugins: [react(), mkcert()],
    test: {
        globals: true,
        environment: "jsdom",
        setupFiles: "./tests/setupTests.ts",
    },
});
