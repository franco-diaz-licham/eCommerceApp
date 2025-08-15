import { Grid } from "@mui/material";
import ProductCard from "./ProductCard";
import type { ProductResponse } from "../types/product.types";

interface ProductListProps {
    products: ProductResponse[];
}

export default function ProductList({ products }: ProductListProps) {
    return (
        <Grid container spacing={3}>
            {products.map((product) => (
                <Grid size={3} display="flex" key={product.id}>
                    <ProductCard product={product} />
                </Grid>
            ))}
        </Grid>
    );
}
