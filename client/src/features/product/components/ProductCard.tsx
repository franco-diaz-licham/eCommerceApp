import { Button, Card, CardActions, CardContent, CardMedia, Typography } from "@mui/material";
import { Link } from "react-router-dom";
// import { useAddBasketItemMutation } from "../basket/basketApi";
import type { ProductResponse } from "../types/product.types";
import { currencyFormat } from "../../../lib/utils";
import { useBasket } from "../../../hooks/useBasket";

/** Functional props. */
interface ProductCardProps {
    product: ProductResponse;
}

export default function ProductCard({ product }: ProductCardProps) {
    const { isAdding, addItemEnsuringBasket } = useBasket();
    const handleAddItem = () => addItemEnsuringBasket(product.id, 1);

    return (
        <Card
            elevation={3}
            sx={{
                width: 280,
                borderRadius: 2,
                display: "flex",
                flexDirection: "column",
                justifyContent: "space-between",
            }}
        >
            <CardMedia sx={{ height: 240, backgroundSize: "cover" }} image={product.photo?.publicUrl} title={product.name} />
            <CardContent>
                <Typography gutterBottom sx={{ textTransform: "uppercase" }} variant="subtitle2">
                    {product.name}
                </Typography>
                <Typography variant="h6" sx={{ color: "secondary.main" }}>
                    {currencyFormat(product.unitPrice)}
                </Typography>
            </CardContent>
            <CardActions sx={{ justifyContent: "space-between" }}>
                <Button disabled={isAdding} onClick={() => handleAddItem()}>
                    Add to cart
                </Button>
                <Button component={Link} to={`/product/${product.id}`}>
                    View
                </Button>
            </CardActions>
        </Card>
    );
}
