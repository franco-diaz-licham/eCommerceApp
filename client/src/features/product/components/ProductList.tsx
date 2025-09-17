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
                <Grid size={{ xs: 12, sm: 6, md: 6, lg: 4, xl: 3 }} display="flex" justifyContent={"center"} key={product.id}>
                    <ProductCard product={product} />
                </Grid>
            ))}
        </Grid>
    );
}
