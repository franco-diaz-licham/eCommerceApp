import { useParams } from "react-router-dom";
import { useFetchProductDetailsQuery } from "../services/product.api";
import { Box, Grid, Button, Chip, Divider, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableRow, TextField, Typography } from "@mui/material";
import { useEffect, useRef, useState, type ChangeEvent } from "react";
import { currencyFormat } from "../../../lib/utils";
import { useBasket } from "../../../hooks/useBasket";
import type { BasketItemResponse } from "../../basket/types/basket.type";
import ProductDetailsSkeleton from "../components/ProductDetailsSkeleton";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";

export default function ProductDetailsPage() {
    const { id } = useParams();
    const productId = id ? Number.parseInt(id) : 0;
    const { getbasketItem, addItemEnsuringBasket, removeItemEnsuringBasket, items } = useBasket();
    const { data: product, isLoading } = useFetchProductDetailsQuery(productId);
    const [item, setItem] = useState<BasketItemResponse | undefined>();
    const [quantity, setQuantity] = useState(1);
    const quantitySetRef = useRef(false);
    const inBasketQty = item?.quantity ?? 0;
    const ctaIsDisabled = quantity === inBasketQty || (!item && quantity === 0);

    useEffect(() => {
        if (!items) return;
        const basketItem = getbasketItem(productId);
        setItem(basketItem);

        if (!basketItem || quantitySetRef.current) return;
        setQuantity(basketItem.quantity);
        quantitySetRef.current = true;
    }, [items, productId, getbasketItem]);

    /** Add or deletes item from the cart. */
    const handleUpdateBasket = async () => {
        const updatedQuantity = item ? Math.abs(quantity - item.quantity) : quantity;
        if (!item || quantity > item.quantity) await addItemEnsuringBasket(productId, updatedQuantity);
        else await removeItemEnsuringBasket(productId, updatedQuantity);
        if (quantity === 0) setQuantity(1);
    };

    /** Update the number of items to be changed in the cart. */
    const handleInputChange = (event: ChangeEvent<HTMLInputElement>) => {
        const value = Number(event.currentTarget.value);
        if (value >= 0) setQuantity(value);
    };

    // --- Skeleton while loading ---
    if (isLoading || !product) return <ProductDetailsSkeleton />;

    // --- Data ready ---
    const productDetails = [
        { label: "Name", value: product.name },
        { label: "Description", value: product.description },
        { label: "Type", value: product.productType?.name },
        { label: "Brand", value: product.brand?.name },
        { label: "Quantity in stock", value: product.quantityInStock },
    ];

    return (
        <Grid container spacing={{ xs: 3, md: 6 }} sx={{ mx: "auto", pt: 4 }}>
            {/* Image panel */}
            <Grid size={{ xs: 12, md: 6 }} display={"flex"} justifyContent="center" alignItems="center">
                <Paper variant="outlined" sx={{ borderRadius: 3, overflow: "hidden", maxWidth: 600 }}>
                    <Box
                        component="img"
                        src={product.photo?.publicUrl}
                        alt={product.name}
                        sx={{
                            width: "100%",
                            height: { xs: 360, md: 800, xl: 800 },
                            objectFit: "cover",
                            display: "block",
                        }}
                    />
                </Paper>
            </Grid>

            {/* Details panel */}
            <Grid size={{ xs: 12, md: 6 }}>
                <Stack spacing={3}>
                    {/* Title + tags */}
                    <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap">
                        <Box sx={{ display: "inline-block", position: "relative", mb: 2 }}>
                            <Typography variant="h4" sx={{ fontWeight: 800, letterSpacing: 0.3 }}>
                                {product.name}
                            </Typography>
                            <Box
                                sx={{
                                    position: "absolute",
                                    bottom: -4,
                                    left: 0,
                                    width: 32,
                                    height: 3,
                                    bgcolor: "primary.main",
                                    borderRadius: 2,
                                }}
                            />
                        </Box>
                        {!!product.brand?.name && <Chip label={product.brand?.name} size="small" />}
                        {!!product.productType?.name && <Chip label={product.productType?.name} size="small" variant="outlined" />}
                    </Stack>

                    <Divider />

                    {/* Price */}
                    <Typography variant="h4" color="secondary" sx={{ fontWeight: 800 }}>
                        {currencyFormat(product.unitPrice)}
                    </Typography>

                    {/* Details table */}
                    <TableContainer component={Paper} variant="outlined" sx={{ borderRadius: 3, overflow: "hidden" }}>
                        <Table size="small" sx={{ "& td": { py: 1.25 } }}>
                            <TableBody>
                                {productDetails.map((d, i) => (
                                    <TableRow
                                        key={d.label}
                                        sx={{
                                            bgcolor: i % 2 === 0 ? "#dadada41" : "inherit",
                                        }}
                                    >
                                        <TableCell sx={{ width: 200, fontWeight: 700 }}>{d.label}</TableCell>
                                        <TableCell>{d.value ?? "-"}</TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>

                    {/* Quantity + CTA */}
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
                            <TextField type="number" label="Quantity in basket" value={quantity} onChange={handleInputChange} fullWidth inputProps={{ min: 0 }} />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6 }}>
                            <Button onClick={handleUpdateBasket} disabled={ctaIsDisabled} color="primary" size="large" variant="contained" fullWidth sx={{ height: 56, textTransform: "none" }}>
                                {item && quantity < inBasketQty ? "Update quantity" : "Add to basket"} <ShoppingCartIcon sx={{ml: 1}}/>
                            </Button>
                        </Grid>
                    </Grid>
                </Stack>
            </Grid>
        </Grid>
    );
}
