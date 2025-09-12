import { Grid, Typography } from "@mui/material";
import BasketItem from "../components/BasketItem";
import { useFetchBasketQuery } from "../services/basketApi";


export default function BasketPage() {
    const {data, isLoading} = useFetchBasketQuery();

    if (isLoading) return <Typography>Loading basket...</Typography>

    if (!data || data.items.length === 0) return <Typography variant="h3">Your basket is empty</Typography>

    return (
        <Grid container spacing={2}>
            <Grid size={8}>
                {data.items.map(item => (
                    <BasketItem item={item} key={item.productId} />
                ))}
            </Grid>
            <Grid size={4}>
                <OrderSummary />
            </Grid>
        </Grid>
    )
}