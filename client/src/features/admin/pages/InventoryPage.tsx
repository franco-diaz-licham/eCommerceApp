import { Box, Button, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from "@mui/material";
import { useState } from "react";
import { useAppDispatch, useAppSelector } from "../../../app/store/store";
import { useFetchProductsQuery } from "../../product/services/product.api";
import { useDeleteProductMutation } from "../services/adminApi";
import ProductForm from "../components/ProductForm";
import { currencyFormat } from "../../../lib/utils";
import { Delete, Edit } from "@mui/icons-material";
import AppPagination from "../../../components/ui/AppPagination";
import type { ProductFormData, ProductResponse } from "../../product/types/product.types";
import { mapToProductFormData, mapToProductResponse } from "../../../lib/mapper";
import { setPageNumber } from "../../product/services/productSlice";

export default function InventoryPage() {
    const productParams = useAppSelector((state) => state.products);
    const { data, refetch } = useFetchProductsQuery(productParams);
    const dispatch = useAppDispatch();
    const [editMode, setEditMode] = useState(false);
    const [selectedProduct, setSelectedProduct] = useState<ProductResponse | null>(null);
    const [deleteProduct] = useDeleteProductMutation();

    /** Handles product selected and sets edit mode. */
    const handleSelectProduct = (product: ProductResponse) => {
        setSelectedProduct(product);
        setEditMode(true);
    };

    /** Handles delete logic. */
    const handleDeleteProduct = async (id: number) => {
        try {
            await deleteProduct(id);
            refetch();
        } catch (error) {
            console.log(error);
        }
    };

    const handleSelectedProduct = (data: ProductFormData | null) => {
        if (!data) return;
        const product = mapToProductResponse(data);
        setSelectedProduct(product);
    };

    if (editMode && selectedProduct) return <ProductForm setEditMode={setEditMode} product={mapToProductFormData(selectedProduct)} refetch={refetch} setSelectedProduct={handleSelectedProduct} />;

    return (
        <>
            <Box display="flex" justifyContent="space-between">
                <Typography sx={{ p: 2 }} variant="h4">
                    Inventory
                </Typography>
                <Button onClick={() => setEditMode(true)} sx={{ m: 2 }} size="large" variant="contained">
                    Create
                </Button>
            </Box>
            <TableContainer component={Paper}>
                <Table sx={{ minWidth: 650 }}>
                    <TableHead>
                        <TableRow>
                            <TableCell>#</TableCell>
                            <TableCell align="left">Product</TableCell>
                            <TableCell align="right">Price</TableCell>
                            <TableCell align="center">Type</TableCell>
                            <TableCell align="center">Brand</TableCell>
                            <TableCell align="center">Quantity</TableCell>
                            <TableCell align="right"></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data &&
                            data.response.map((product) => (
                                <TableRow
                                    key={product.id}
                                    sx={{
                                        "&:last-child td, &:last-child th": { border: 0 },
                                    }}
                                >
                                    <TableCell component="th" scope="row">
                                        {product.id}
                                    </TableCell>
                                    <TableCell align="left">
                                        <Box display="flex" alignItems="center">
                                            <img src={product.photo?.publicUrl} alt={product.name} style={{ height: 50, marginRight: 20 }} />
                                            <span>{product.name}</span>
                                        </Box>
                                    </TableCell>
                                    <TableCell align="right">{currencyFormat(product.unitPrice)}</TableCell>
                                    <TableCell align="center">{product.productType?.name}</TableCell>
                                    <TableCell align="center">{product.brand?.name}</TableCell>
                                    <TableCell align="center">{product.quantityInStock}</TableCell>
                                    <TableCell align="right">
                                        <Button onClick={() => handleSelectProduct(product)} startIcon={<Edit />} />
                                        <Button onClick={() => handleDeleteProduct(product.id)} startIcon={<Delete />} color="error" />
                                    </TableCell>
                                </TableRow>
                            ))}
                    </TableBody>
                </Table>
                <Box sx={{ p: 3 }}>{data?.pagination && data.response.length > 0 && <AppPagination metadata={data.pagination} onPageChange={(page: number) => dispatch(setPageNumber(page))} />}</Box>
            </TableContainer>
        </>
    );
}
