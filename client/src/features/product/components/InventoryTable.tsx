import { Delete, Edit } from "@mui/icons-material";
import { Box, IconButton, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import { currencyFormat } from "../../../lib/utils";
import AppPagination from "../../../components/ui/AppPagination";
import type { PaginatedProductsData, ProductResponse } from "../models/product.types";

interface InventoryPropsTable {
    data?: PaginatedProductsData;
    onProductSelected: (product: ProductResponse) => void;
    onProductDeleted: (product: number) => void;
    onPaginationChanged: (page: number) => void;
}

export default function InventoryTable(props: InventoryPropsTable) {
    return (
        <TableContainer component={Paper} sx={{ borderRadius: 3, overflow: "hidden" }}>
            <Table>
                <TableHead>
                    <TableRow>
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
                    {props.data?.response.map((product) => (
                        <TableRow key={product.id} hover>
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
                                <IconButton onClick={() => props.onProductSelected(product)} color="primary">
                                    <Edit />
                                </IconButton>
                                <IconButton onClick={() => props.onProductDeleted(product.id)} color="error">
                                    <Delete />
                                </IconButton>
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
            {props.data?.pagination && props.data.response.length > 0 && (
                <Box
                    sx={{
                        p: 2,
                        display: "flex",
                        justifyContent: "center",
                    }}
                >
                    <AppPagination metadata={props.data.pagination} onPageChange={(page: number) => props.onPaginationChanged(page)} />
                </Box>
            )}
        </TableContainer>
    );
}
