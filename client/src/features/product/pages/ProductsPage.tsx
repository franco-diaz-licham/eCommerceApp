import { Typography } from "@mui/material";
import ProductList from "../components/ProductList";
import { useFetchProductsQuery } from "../services/product.api";
import type { ProductQueryParams } from "../types/product.types";

export default function ProductsPage() {
    const query: ProductQueryParams = { orderBy: "nameAsc" };
    const { data : products, isLoading } = useFetchProductsQuery(query);
    if (isLoading || !products) return <div>Loading...</div>;

    return (
        <>
            <Typography variant="h4" sx={{ fontWeight: "bold" }}>
                Product Page
            </Typography>
            <ProductList products={products.response} />
        </>
    );
}
