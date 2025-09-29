import { Chip, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import type { OrderResponse, PaginatedOrdersData } from "../models/order.type";
import { currencyFormat, formatDate } from "../../../lib/utils";
import { AccessTime, Cancel, LocalShipping, TaskAlt } from "@mui/icons-material";
// import AppPagination from "../../../components/ui/AppPagination";

interface OrdersTableProps {
    orders: PaginatedOrdersData;
    OnOrderClicked: (order: number) => void;
    // onPaginationChanged?: (page: number) => void; // optional: match InventoryTable API
}

export default function OrdersTable(props: OrdersTableProps) {
    const { orders, OnOrderClicked } = props;
    const getStatus = (order: OrderResponse) => {
        if (order.orderStatus.id === 1) return <Chip size="small" label={order.orderStatus.name} color="default" icon={<AccessTime />} />;
        if (order.orderStatus.id === 3) return <Chip size="small" label={order.orderStatus.name} color="primary" icon={<LocalShipping />} />;
        if (order.orderStatus.id === 5) return <Chip size="small" label={order.orderStatus.name} color="error" icon={<Cancel />} />;
        return <Chip size="small" label={order.orderStatus.name} color="success" icon={<TaskAlt />} />;
    };

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
                        <TableRow key={order.id} hover onClick={() => OnOrderClicked(order.id)} sx={{ cursor: "pointer" }}>
                            <TableCell align="center">{order.id}</TableCell>
                            <TableCell align="center">{formatDate(order.orderDate)}</TableCell>
                            <TableCell align="center">{currencyFormat(order.total)}</TableCell>
                            <TableCell align="center">{getStatus(order)}</TableCell>
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
