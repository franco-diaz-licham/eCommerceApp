import { createBrowserRouter, Navigate } from "react-router-dom";
import App from "../App";
import RequireAuth from "./RequireAuth";
import AboutPage from "../../pages/AboutPage";
import HomePage from "../../pages/HomePage";
import ProductDetailsPage from "../../features/product/pages/ProductDetailsPage";
import ProductsPage from "../../features/product/pages/ProductsPage";
import ContactPage from "@mui/icons-material/ContactPage";
import BasketPage from "../../features/basket/pages/BasketPage";
import NotFoundPage from "../../features/error/pages/NotFoundPage";
import ErrorPage from "../../features/error/pages/ErrorPage";
import ServerErrorPage from "../../features/error/pages/ServerErrorPage";
import InventoryPage from "../../features/product/pages/InventoryPage";

export const Routes = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            // {
            //     element: <RequireAuth />,
            //     children: [
            //         { path: "checkout", element: <CheckoutPage /> },
            //         { path: "checkout/success", element: <CheckoutSuccess /> },
            //         { path: "orders", element: <OrdersPage /> },
            //         { path: "orders/:id", element: <OrderDetailedPage /> },
            //         { path: "inventory", element: <InventoryPage /> },
            //     ],
            // },
            { path: "", element: <HomePage /> },
            { path: "products", element: <ProductsPage /> },
            { path: "product/:id", element: <ProductDetailsPage /> },
            { path: "about", element: <AboutPage /> },
            { path: "contact", element: <ContactPage /> },
            { path: "basket", element: <BasketPage /> },
            { path: "errors", element: <ErrorPage /> },
            // { path: "login", element: <LoginForm /> },
            // { path: "register", element: <RegisterForm /> },
            { path: "inventory", element: <InventoryPage /> },
            { path: "server-error", element: <ServerErrorPage /> },
            { path: "not-found", element: <NotFoundPage /> },
            { path: "*", element: <Navigate replace to="/not-found" /> },
        ],
    },
]);
