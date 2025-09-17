import { Button, Grid, Typography } from "@mui/material";
import BasketItem from "../components/BasketItem";
import OrderSummary from "../../order/components/OrderSummary";
import { useBasket } from "../../../hooks/useBasket";

export default function BasketPage() {
    const { basket, isLoading, clearBasket } = useBasket();

    if (isLoading) return <Typography>Loading basket...</Typography>;

    if (!basket || basket.basketItems.length === 0) return <Typography variant="h3">Your basket is empty</Typography>;

    return (
        <Grid container spacing={2}>
            <Grid size={12}>
                <Grid container spacing={2}>
                    <Grid size={6}>
                        <Typography variant="h4" sx={{ fontWeight: "bold" }}>
                            Basket
                        </Typography>
                    </Grid>
                    <Grid size={2}>
                        {!location.pathname.includes("checkout") && (
                            <Button variant="contained" color="primary" onClick={() => clearBasket()} fullWidth sx={{ mb: 1 }}>
                                Clear All
                            </Button>
                        )}
                    </Grid>
                </Grid>
            </Grid>
            <Grid size={8}>
                {basket.basketItems.map((item) => (
                    <BasketItem item={item} key={item.productId} />
                ))}
            </Grid>
            <Grid size={4}>
                <OrderSummary />
            </Grid>
        </Grid>
    );
}
