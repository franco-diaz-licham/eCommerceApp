import { useParams } from "react-router-dom";
import { useFetchProductDetailsQuery } from "../services/product.api";
import { Divider, Grid, Table, TableBody, TableCell, TableContainer, TableRow, TextField, Typography } from "@mui/material";
import { useState, type ChangeEvent } from "react";

export default function ProductDetailsPage() {
    const { id } = useParams();
    const [quantity, setQuantity] = useState(1);

    const { data: product, isLoading } = useFetchProductDetailsQuery(id ? Number.parseInt(id) : 0);
    if (!product || isLoading) return <div>Loading...</div>;

    // const handleUpdateBasket = () => {
    //     const updatedQuantity = item ? Math.abs(quantity - item.quantity) : quantity;
    //     if (!item || quantity > item.quantity) {
    //         addBasketItem({ product, quantity: updatedQuantity });
    //     } else {
    //         removeBasketItem({ productId: product.id, quantity: updatedQuantity });
    //     }
    // };

    const handleInputChange = (event: ChangeEvent<HTMLInputElement>) => {
        const value = +event.currentTarget.value;
        if (value >= 0) setQuantity(value);
    };

    const productDetails = [
        { label: "Name", value: product?.name },
        { label: "Description", value: product.description },
        { label: "Type", value: product.productType?.name },
        { label: "Brand", value: product.brand?.name },
        { label: "Quantity in stock", value: product.quantityInStock },
    ];

    return (
        <Grid container spacing={6} maxWidth="lg" sx={{ mx: "auto" }}>
            <Grid size={6}>
                <img src={product?.photo?.publicUrl} alt={product.name} style={{ width: "100%" }} />
            </Grid>
            <Grid size={6}>
                <Typography variant="h3">{product.name}</Typography>
                <Divider sx={{ mb: 2 }} />
                <Typography variant="h4" color="secondary">
                    ${(product.price / 100).toFixed(2)}
                </Typography>
                <TableContainer>
                    <Table
                        sx={{
                            "& td": { fontSize: "1rem" },
                        }}
                    >
                        <TableBody>
                            {productDetails.map((detail, index) => (
                                <TableRow key={index}>
                                    <TableCell sx={{ fontWeight: "bold" }}>{detail.label}</TableCell>
                                    <TableCell>{detail.value}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
                <Grid container spacing={2} marginTop={3}>
                    <Grid size={6}>
                        <TextField variant="outlined" type="number" label="Quantity in basket" fullWidth value={quantity} onChange={handleInputChange} />
                    </Grid>
                    <Grid size={6}>
                        {/* <Button onClick={handleUpdateBasket} disabled={quantity === item?.quantity || (!item && quantity === 0)} sx={{ height: "55px" }} color="primary" size="large" variant="contained" fullWidth>
                            {item ? "Update quantity" : "Add to basket"}
                        </Button> */}
                    </Grid>
                </Grid>
            </Grid>
        </Grid>
    );
}
