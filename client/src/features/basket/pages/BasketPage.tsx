import { Box, Button, Grid, Typography } from "@mui/material";
import { Link } from "react-router-dom";
import BasketItem from "../components/BasketItem";
import OrderSummary from "../../order/components/OrderSummary";
import { useBasket } from "../../../hooks/useBasket";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import Header from "../../../components/ui/Header";
import ShoppingCartOutlinedIcon from "@mui/icons-material/ShoppingCartOutlined";

export default function BasketPage() {
    const { basket, subtotal, discount, deliveryFee, total, isLoading, clearBasket, removeItemEnsuringBasket, addItemEnsuringBasket } = useBasket();

    /** Add selected item to the curernt basket. */
    const handleAddItem = async (itemId: number, numb: number): Promise<void> => {
        await addItemEnsuringBasket(itemId, numb);
    };

    /** Remove selected item from the current basket. */
    const handleRemoveItem = async (itemId: number, numb: number): Promise<void> => {
        removeItemEnsuringBasket(itemId, numb);
    };

    if (isLoading) return;

    if (!basket || basket.basketItems.length === 0) {
        return (
            <Box
                sx={{
                    height: "calc(100vh - 300px)",
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    alignItems: "center",
                }}
            >
                <ShoppingCartOutlinedIcon sx={{ fontSize: 100 }} color="primary" />
                <Typography gutterBottom variant="h3">
                    Your basket is empty
                </Typography>
                <Button component={Link} to="/products" size="large" variant="contained">
                    Go back to shop
                </Button>
            </Box>
        );
    }

    return (
        <Box sx={{ pt: 4 }}>
            {/* Header */}
            <Header title="Basket">
                {!location.pathname.includes("checkout") && (
                    <Button onClick={() => clearBasket()} variant="contained" sx={{ textTransform: "none" }} color="error" size="small" disabled={basket && basket.basketItems.length > 0 ? false : true}>
                        Clear All
                        <DeleteForeverIcon sx={{ ml: 1 }} />
                    </Button>
                )}
            </Header>

            {/* Content */}
            {!isLoading && (
                <Grid container spacing={2}>
                    <Grid size={8}>
                        {basket?.basketItems.map((item) => (
                            <BasketItem item={item} key={item.productId} onAdditemChanged={(numb) => handleAddItem(item.productId, numb)} onRemoveItem={(numb) => handleRemoveItem(item.productId, numb)} />
                        ))}
                    </Grid>
                    <Grid size={4}>
                        <OrderSummary subtotal={subtotal} discount={discount} deliveryFee={deliveryFee} total={total} inCheckout={false} />
                    </Grid>
                </Grid>
            )}
        </Box>
    );
}
