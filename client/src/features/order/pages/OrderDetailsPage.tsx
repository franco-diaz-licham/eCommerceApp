import { Link, useParams } from "react-router-dom";
import { Box, Button, Divider, Paper, Table, TableBody, TableCell, TableContainer, TableRow, Typography } from "@mui/material";
import { currencyFormat, formatAddressString, formatDate, formatPaymentString } from "../../../lib/utils";
import { useFetchOrderDetailedQuery } from "../api/orderApi";
import Header from "../../../components/ui/Header";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import WarningAmberIcon from '@mui/icons-material/WarningAmber';

export default function OrderDetailedPage() {
    const { id } = useParams();
    const { data: order, isLoading } = useFetchOrderDetailedQuery(+id!);

    if (isLoading) return;
    if (!order) {
        return (
            <Box
                sx={{
                    height: "calc(100vh - 300px)",
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    alignItems: "center",
                }}
            >
                <WarningAmberIcon sx={{ fontSize: 100 }} color="primary" />
                <Typography gutterBottom variant="h3">
                    Order Not Found
                </Typography>
                <Button component={Link} to="/orders" size="large" variant="contained">
                    Go back to orders
                </Button>
            </Box>
        );
    }

    return (
        <Box sx={{ pt: 4 }}>
            {/* Header */}
            <Header title="My Orders">
                <Button component={Link} to="/orders" variant="contained" size="small" sx={{ textTransform: "none" }}>
                    Back to orders
                    <ArrowBackIcon sx={{ ml: 1 }} />
                </Button>
            </Header>
            <Box component={Paper} sx={{ p: 4 }}>
                <Box>
                    <Typography variant="h6" fontWeight="bold">
                        Billing and delivery information
                    </Typography>
                    <Box component="dl">
                        <Typography component="dt" variant="subtitle1" fontWeight="500">
                            Shipping address
                        </Typography>
                        <Typography component="dd" variant="body2" fontWeight="300">
                            {formatAddressString(order.shippingAddress.line1, order.shippingAddress.city, order.shippingAddress.state, order.shippingAddress.postalCode, order.shippingAddress.country)}
                        </Typography>
                    </Box>
                    <Box component="dl">
                        <Typography component="dt" variant="subtitle1" fontWeight="500">
                            Payment info
                        </Typography>
                        <Typography component="dd" variant="body2" fontWeight="300">
                            {formatPaymentString(order.paymentSummary.brand, String(order.paymentSummary.last4), order.paymentSummary.expMonth, order.paymentSummary.expYear)}
                        </Typography>
                    </Box>
                </Box>

                <Divider sx={{ my: 2 }} />

                <Box>
                    <Typography variant="h6" fontWeight="bold">
                        Order details
                    </Typography>
                    <Box component="dl">
                        <Typography component="dt" variant="subtitle1" fontWeight="500">
                            Name
                        </Typography>
                        <Typography component="dd" variant="body2" fontWeight="300">
                            {order.shippingAddress.recipientName}
                        </Typography>
                    </Box>
                    <Box component="dl">
                        <Typography component="dt" variant="subtitle1" fontWeight="500">
                            Order status
                        </Typography>
                        <Typography component="dd" variant="body2" fontWeight="300">
                            {order.orderStatus.name}
                        </Typography>
                    </Box>
                    <Box component="dl">
                        <Typography component="dt" variant="subtitle1" fontWeight="500">
                            Order date
                        </Typography>
                        <Typography component="dd" variant="body2" fontWeight="300">
                            {formatDate(order.orderDate)}
                        </Typography>
                    </Box>
                </Box>

                <Divider sx={{ my: 2 }} />

                <TableContainer>
                    <Table>
                        <TableBody>
                            {order.orderItems.map((item) => (
                                <TableRow key={item.productId} sx={{ borderBottom: "1px solid rgba(224, 224, 224, 1)" }}>
                                    <TableCell sx={{ py: 4 }}>
                                        <Box display="flex" gap={3} alignItems="center">
                                            <img src={item.pictureUrl} alt={item.productName} style={{ width: 40, height: 40 }} />
                                            <Typography>{item.productName}</Typography>
                                        </Box>
                                    </TableCell>
                                    <TableCell align="center" sx={{ p: 4 }}>
                                        x {item.quantity}
                                    </TableCell>
                                    <TableCell align="right" sx={{ p: 4 }}>
                                        {currencyFormat(item.unitPrice * item.quantity)}
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>

                <Box mx={3}>
                    <Box component="dl" display="flex" justifyContent="space-between">
                        <Typography component="dt" variant="subtitle1" fontWeight="500">
                            Subtotal
                        </Typography>
                        <Typography component="dd" variant="body2" fontWeight="300">
                            {currencyFormat(order.subtotal)}
                        </Typography>
                    </Box>
                    <Box component="dl" display="flex" justifyContent="space-between">
                        <Typography component="dt" variant="subtitle1" fontWeight="500">
                            Discount
                        </Typography>
                        <Typography component="dd" variant="body2" fontWeight="300" color="green">
                            {currencyFormat(order.discount)}
                        </Typography>
                    </Box>
                    <Box component="dl" display="flex" justifyContent="space-between">
                        <Typography component="dt" variant="subtitle1" fontWeight="500">
                            Delivery fee
                        </Typography>
                        <Typography component="dd" variant="body2" fontWeight="300">
                            {currencyFormat(order.deliveryFee)}
                        </Typography>
                    </Box>
                </Box>
                <Box component="dl" display="flex" justifyContent="space-between" mx={3}>
                    <Typography component="dt" variant="subtitle1" fontWeight="500">
                        Total
                    </Typography>
                    <Typography component="dd" variant="body2" fontWeight="700">
                        {currencyFormat(order.total)}
                    </Typography>
                </Box>
            </Box>
        </Box>
    );
}
