import { Box, Grid, IconButton, Paper, Typography } from "@mui/material";
import { Add, DeleteOutline, Remove } from "@mui/icons-material";
import { currencyFormat } from "../../../lib/utils";
import type { BasketItemResponse } from "../types/basket.type";

interface BasketItemProps {
    item: BasketItemResponse;
    onRemoveItem: (numb: number) => Promise<void>;
    onAdditemChanged: (number: number) => Promise<void>;
}

export default function BasketItem(props: BasketItemProps) {
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
                    src={props.item.publicUrl}
                    alt={props.item.name}
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
                    <Typography variant="h6">{props.item.name}</Typography>

                    <Box display="flex" alignItems="center" gap={3}>
                        <Typography sx={{ fontSize: "1.1rem" }}>
                            {currencyFormat(props.item.unitPrice)} x {props.item.quantity}
                        </Typography>
                        <Typography sx={{ fontSize: "1.1rem" }}>=</Typography>
                        <Typography sx={{ fontSize: "1.1rem" }} color="primary">
                            {currencyFormat(props.item.lineTotal)}
                        </Typography>
                    </Box>

                    <Grid container spacing={1} alignItems="center">
                        <IconButton onClick={() => props.onRemoveItem(1)} color="error" size="small" sx={{ border: 1, borderRadius: 1, minWidth: 0 }}>
                            <Remove />
                        </IconButton>
                        <Typography variant="h6">{props.item.quantity}</Typography>
                        <IconButton onClick={() => props.onAdditemChanged(1)} color="success" size="small" sx={{ border: 1, borderRadius: 1, minWidth: 0 }}>
                            <Add />
                        </IconButton>
                    </Grid>
                </Box>
            </Box>
            <IconButton
                onClick={() => props.onRemoveItem(props.item.quantity)}
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
                <DeleteOutline />
            </IconButton>
        </Paper>
    );
}
