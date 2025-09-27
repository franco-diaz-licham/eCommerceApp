import { Box, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import type { PaginatedOrdersData } from "../models/order.type";
import { currencyFormat, formatDate } from "../../../lib/utils";
import AppPagination from "../../../components/ui/AppPagination";
// If you want pagination like InventoryTable, uncomment the next line and the block at the bottom.
// import AppPagination from "../../../components/ui/AppPagination";

interface OrdersTableProps {
    orders: PaginatedOrdersData;
    OnOrderClicked: (order: number) => void;
    // onPaginationChanged?: (page: number) => void; // optional: match InventoryTable API
}

export default function OrdersTable(props: OrdersTableProps) {
    const { orders, OnOrderClicked } = props;

    return (
        <TableContainer component={Paper} sx={{ borderRadius: 3, overflow: "hidden" }}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell align="center">#</TableCell>
                        <TableCell align="center">Date</TableCell>
                        <TableCell align="center">Total</TableCell>
                        <TableCell align="center">Status</TableCell>
                    </TableRow>
                </TableHead>

                <TableBody>
                    {orders.response.map((order) => (
                        <TableRow key={order.id} hover onClick={() => OnOrderClicked(order.id)} sx={{ cursor: "pointer"}}>
                            <TableCell align="center">{order.id}</TableCell>
                            <TableCell align="center">{formatDate(order.orderDate)}</TableCell>
                            <TableCell align="center">{currencyFormat(order.total)}</TableCell>
                            <TableCell align="center">{order.orderStatus.name}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>

            {/* {orders.pagination && orders.response.length > 0 && props.onPaginationChanged && (
                <Box sx={{ p: 2, display: "flex", justifyContent: "center" }}>
                    <AppPagination metadata={orders.pagination} onPageChange={(page: number) => props.onPaginationChanged!(page)} />
                </Box>
            )} */}
        </TableContainer>
    );
}
