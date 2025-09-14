import { Box, Grid, IconButton, Paper, Typography } from "@mui/material";
import { Add, Close, Remove } from "@mui/icons-material";
import { currencyFormat } from "../../../lib/utils";
import type { BasketItemResponse } from "../types/basket.type";
import { useBasket } from "../../../hooks/useBasket";

interface BasketItemProps {
    item: BasketItemResponse;
}

export default function BasketItem({ item }: BasketItemProps) {
    const { removeItemEnsuringBasket, addItemEnsuringBasket } = useBasket();

    return (
        <Paper
            sx={{
                height: 140,
                borderRadius: 3,
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                mb: 2,
            }}
        >
            <Box display="flex" alignItems="center">
                <Box
                    component="img"
                    src={item.publicUrl}
                    alt={item.name}
                    sx={{
                        width: 100,
                        height: 100,
                        objectFit: "cover",
                        borderRadius: "4px",
                        mr: 8,
                        ml: 4,
                    }}
                />

                <Box display="flex" flexDirection="column" gap={1}>
                    <Typography variant="h6">{item.name}</Typography>

                    <Box display="flex" alignItems="center" gap={3}>
                        <Typography sx={{ fontSize: "1.1rem" }}>
                            {currencyFormat(item.unitPrice)} x {item.quantity}
                        </Typography>
                        <Typography sx={{ fontSize: "1.1rem" }} color="primary">
                            {currencyFormat(item.lineTotal)}
                        </Typography>
                    </Box>

                    <Grid container spacing={1} alignItems="center">
                        <IconButton onClick={() => removeItemEnsuringBasket(item.productId, 1)} color="error" size="small" sx={{ border: 1, borderRadius: 1, minWidth: 0 }}>
                            <Remove />
                        </IconButton>
                        <Typography variant="h6">{item.quantity}</Typography>
                        <IconButton onClick={() => addItemEnsuringBasket(item.productId, 1)} color="success" size="small" sx={{ border: 1, borderRadius: 1, minWidth: 0 }}>
                            <Add />
                        </IconButton>
                    </Grid>
                </Box>
            </Box>
            <IconButton
                onClick={() => removeItemEnsuringBasket(item.productId, item.quantity)}
                color="error"
                size="small"
                sx={{
                    border: 1,
                    borderRadius: 1,
                    minWidth: 0,
                    alignSelf: "start",
                    mr: 1,
                    mt: 1,
                }}
            >
                <Close />
            </IconButton>
        </Paper>
    );
}
