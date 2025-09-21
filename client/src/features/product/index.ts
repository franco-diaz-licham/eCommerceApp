// Pages (export only the ones you want routed externally)
export { default as ProductsPage } from "./pages/ProductsPage";
export { default as ProductDetailsPage } from "./pages/ProductDetailsPage";
export { default as InventoryPage } from "./pages/InventoryPage";

// Components that might be reused elsewhere
export { ProductForm } from "./components/productForm";
export { default as ProductCard } from "./components/ProductCard";
export { default as ProductList } from "./components/ProductList";

// Types that are safe to use outside
export type { ProductCreate } from "./models/product.types";
