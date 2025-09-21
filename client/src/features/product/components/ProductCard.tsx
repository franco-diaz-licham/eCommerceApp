import { Button, Card, CardActions, CardContent, CardMedia, Typography } from "@mui/material";
import { Link } from "react-router-dom";
import { currencyFormat } from "../../../lib/utils";

export type ProductCardDto = {
    id: number;
    name: string;
    unitPrice: number;
    photoUrl?: string | null;
};

type ProductCardProps = {
    product: ProductCardDto;
    isAdding?: boolean;
    onAddToCart: (productId: number) => void;
};

export default function ProductCard({ product, isAdding, onAddToCart }: ProductCardProps) {
    return (
        <Card
            elevation={3}
            sx={{
                borderRadius: 3,
                border: (t) => `1px solid ${t.palette.divider}`,
                "&:hover": { boxShadow: 4, transform: "translateY(-2px)" },
                transition: "all .18s ease",
                width: 280,
            }}
        >
            <CardMedia sx={{ height: 240, backgroundSize: "cover" }} image={product.photoUrl || undefined} title={product.name} />
            <CardContent>
                <Typography gutterBottom sx={{ textTransform: "uppercase" }} variant="subtitle2">
                    {product.name}
                </Typography>
                <Typography variant="h6" sx={{ color: "secondary.main" }} fontWeight={700}>
                    {currencyFormat(product.unitPrice)}
                </Typography>
            </CardContent>
            <CardActions sx={{ justifyContent: "space-between" }}>
                <Button disabled={!!isAdding} onClick={() => onAddToCart(product.id)} variant="contained" size="small">
                    Add to cart
                </Button>
                <Button component={Link} to={`/product/${product.id}`}>
                    View
                </Button>
            </CardActions>
        </Card>
    );
}
