import { Box, Button, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from "@mui/material";
import { useState } from "react";
import { useAppDispatch, useAppSelector } from "../../../app/store/store";
import { useCreateProductMutation, useDeleteProductMutation, useFetchProductsQuery, useUpdateProductMutation } from "../services/product.api";
import ProductForm from "../components/ProductForm";
import { currencyFormat } from "../../../lib/utils";
import { Delete, Edit } from "@mui/icons-material";
import AppPagination from "../../../components/ui/AppPagination";
import type { ProductFormData, ProductResponse } from "../types/product.types";
import { mapToProductCreate, mapToProductFormData, mapToProductUpdate } from "../../../lib/mapper";
import { setPageNumber } from "../services/productSlice";

export default function InventoryPage() {
    const productParams = useAppSelector((state) => state.products);
    const { data, refetch } = useFetchProductsQuery(productParams);
    const dispatch = useAppDispatch();
    const [editMode, setEditMode] = useState(false);
    const [selectedProduct, setSelectedProduct] = useState<ProductResponse | null>(null);
    const [deleteProduct] = useDeleteProductMutation();
    const [createProduct] = useCreateProductMutation();
    const [updateProduct] = useUpdateProductMutation();

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

    /** Handles on submit opperations. */
    const handleOnSubmit = async (data: ProductFormData) => {
        try {
            if (data?.id) {
                const product = mapToProductUpdate(data);
                await updateProduct(product).unwrap();
            } else {
                const product = mapToProductCreate(data);
                await createProduct(product).unwrap();
            }

            setEditMode(false);
            setSelectedProduct(null);
            refetch();
        } catch (error) {
            console.log(error);
        }
    };

    /** Resets state when cancelled. */
    const handleCancel = () => {
        setSelectedProduct(null);
        setEditMode(false);
    };

    if (editMode) return <ProductForm onFormCancel={handleCancel} product={selectedProduct ? mapToProductFormData(selectedProduct) : null} onFormSubmit={handleOnSubmit} />;

    return (
        <>
            <Box display="flex" justifyContent="space-between">
                <Typography variant="h4" sx={{ fontWeight: "bold" }}>
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
