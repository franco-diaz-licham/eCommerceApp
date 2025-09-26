import { Grid } from "@mui/material";
import ProductCard, { type ProductCardModel } from "./ProductCard";

type ProductListProps = {
    products: ProductCardModel[];
    isAdding?: boolean;
    onAddToCart: (productId: number) => void;
};

export default function ProductList({ products, isAdding, onAddToCart }: ProductListProps) {
    return (
        <Grid container spacing={3}>
            {products.map((product) => (
                <Grid size={{ xs: 12, sm: 6, md: 6, lg: 4, xl: 3 }} display="flex" justifyContent={"center"} key={product.id}>
                    <ProductCard product={product} isAdding={!!isAdding} onAddToCart={onAddToCart} />
                </Grid>
            ))}
        </Grid>
    );
}
