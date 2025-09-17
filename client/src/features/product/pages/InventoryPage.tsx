import { Box, Button, IconButton, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from "@mui/material";
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

    const handleSelectProduct = (product: ProductResponse) => {
        setSelectedProduct(product);
        setEditMode(true);
    };

    const handleDeleteProduct = async (id: number) => {
        try {
            await deleteProduct(id);
            refetch();
        } catch (error) {
            console.log(error);
        }
    };

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

    const handleCancel = () => {
        setSelectedProduct(null);
        setEditMode(false);
    };

    if (editMode) return <ProductForm onFormCancel={handleCancel} product={selectedProduct ? mapToProductFormData(selectedProduct) : null} onFormSubmit={handleOnSubmit} />;
    return (
        <>
            {/* Header */}
            <Box
                sx={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    mb: 3,
                    pt: 4,
                }}
            >
                <Box sx={{ display: "inline-block", position: "relative", mb: 2 }}>
                    <Typography variant="h4" sx={{ fontWeight: 800 }}>
                        Inventory
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

                <Button onClick={() => setEditMode(true)} size="large" variant="contained">
                    Create
                </Button>
            </Box>

            {/* Table */}
            <TableContainer component={Paper} sx={{ borderRadius: 3, overflow: "hidden" }}>
                <Table>
                    <TableHead>
                        <TableRow sx={{ bgcolor: "grey.100" }}>
                            <TableCell>#</TableCell>
                            <TableCell>Product</TableCell>
                            <TableCell align="right">Price</TableCell>
                            <TableCell align="center">Type</TableCell>
                            <TableCell align="center">Brand</TableCell>
                            <TableCell align="center">Quantity</TableCell>
                            <TableCell align="right">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data?.response.map((product) => (
                            <TableRow
                                key={product.id}
                                hover
                                sx={{
                                    "&:nth-of-type(odd)": { bgcolor: "grey.50" },
                                }}
                            >
                                <TableCell>{product.id}</TableCell>
                                <TableCell>
                                    <Box display="flex" alignItems="center">
                                        <Box
                                            component="img"
                                            src={product.photo?.publicUrl}
                                            alt={product.name}
                                            sx={{
                                                width: 50,
                                                height: 50,
                                                objectFit: "cover",
                                                borderRadius: 1,
                                                mr: 2,
                                            }}
                                        />
                                        {product.name}
                                    </Box>
                                </TableCell>
                                <TableCell align="right">{currencyFormat(product.unitPrice)}</TableCell>
                                <TableCell align="center">{product.productType?.name}</TableCell>
                                <TableCell align="center">{product.brand?.name}</TableCell>
                                <TableCell align="center">{product.quantityInStock}</TableCell>
                                <TableCell align="right">
                                    <IconButton onClick={() => handleSelectProduct(product)} color="primary">
                                        <Edit />
                                    </IconButton>
                                    <IconButton onClick={() => handleDeleteProduct(product.id)} color="error">
                                        <Delete />
                                    </IconButton>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
                {data?.pagination && data.response.length > 0 && (
                    <Box
                        sx={{
                            p: 2,
                            display: "flex",
                            justifyContent: "center",
                        }}
                    >
                        <AppPagination metadata={data.pagination} onPageChange={(page: number) => dispatch(setPageNumber(page))} />
                    </Box>
                )}
            </TableContainer>
        </>
    );
}
