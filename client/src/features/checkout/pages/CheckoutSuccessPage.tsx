import { Box, Button, Container, Divider, Paper, Typography, Stack } from "@mui/material";
import { Link, useLocation } from "react-router-dom";
import CheckCircleOutlineRoundedIcon from "@mui/icons-material/CheckCircleOutlineRounded";
import ReceiptLongOutlinedIcon from "@mui/icons-material/ReceiptLongOutlined";
import { currencyFormat, formatAddressString, formatPaymentString } from "../../../lib/utils";
import type { OrderResponse } from "../../order/models/order.type";

/** Helper method to get order response. */
function useOrderFromLocation(): OrderResponse | undefined {
    const { state } = useLocation();
    const s = state as unknown;
    
    if (s && typeof s === "object") {
        if ("id" in (s as Record<string, unknown>) && "shippingAddress" in (s as Record<string, unknown>)) {
            return s as OrderResponse;
        }
    }
    return undefined;
}

export default function CheckoutSuccessPage() {
    const order = useOrderFromLocation();

    if (!order) {
        return (
            <Container maxWidth="sm" sx={{ py: 8 }}>
                <Box
                    sx={{
                        height: "calc(100vh - 300px)",
                        display: "flex",
                        flexDirection: "column",
                        justifyContent: "center",
                        alignItems: "center",
                        textAlign: "center",
                        gap: 2,
                    }}
                >
                    <ReceiptLongOutlinedIcon sx={{ fontSize: 96 }} color="error" />
                    <Typography variant="h4" fontWeight={700}>
                        Oops — we couldn’t find your order
                    </Typography>
                    <Typography color="text.secondary">Try returning to the shop and checking out again.</Typography>
                    <Button component={Link} to="/products" size="large" variant="contained" sx={{ mt: 1 }}>
                        Go back to shop
                    </Button>
                </Box>
            </Container>
        );
    }

    return (
        <Container
            sx={{
                height: "calc(100vh - 300px)",
                display: "flex",
                flexDirection: "column",
                justifyContent: "center",
                alignItems: "center",
                textAlign: "center",
                gap: 2,
            }}
        >
            {/* Hero */}
            <Box
                sx={{
                    display: "flex",
                    alignItems: "center",
                    gap: 2,
                    mb: 3,
                    justifyContent: "center",
                    textAlign: "center",
                    flexDirection: "column",
                }}
            >
                <CheckCircleOutlineRoundedIcon sx={{ fontSize: 96 }} color="success" />
                <Stack spacing={0.5}>
                    <Typography variant="h4" fontWeight={800}>
                        Thanks for your order!
                    </Typography>
                    <Typography variant="body1" color="text.secondary">
                        Order <strong>#{order.id}</strong> was placed on <strong>{order.orderDate}</strong>.
                    </Typography>
                </Stack>
            </Box>

            {/* Summary card */}
            <Paper
                elevation={1}
                sx={{
                    p: { xs: 2, md: 3, lg: 5 },
                    mb: 4,
                    borderRadius: 3,
                    minWidth: 600,
                }}
            >
                <Stack spacing={2}>
                    <Row label="Payment method" value={formatPaymentString(order.paymentSummary.brand, String(order.paymentSummary.last4), order.paymentSummary.expMonth, order.paymentSummary.expYear)} />
                    <Divider />
                    <Row label="Recipient" value={order.shippingAddress.recipientName} />
                    <Divider />
                    <Row label="Shipping address" value={formatAddressString(order.shippingAddress.line1, order.shippingAddress.city, order.shippingAddress.state, order.shippingAddress.postalCode, order.shippingAddress.country)} />
                    <Divider />
                    <Row label="Amount paid" value={currencyFormat(order.total)} />
                </Stack>
            </Paper>

            {/* Actions */}
            <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                <Button variant="contained" color="primary" component={Link} to={`/orders/${order.id}`} size="large">
                    View your order
                </Button>
                <Button component={Link} to="/products" variant="outlined" color="primary" size="large">
                    Continue shopping
                </Button>
            </Stack>
        </Container>
    );
}

/** Tiny helper row */
function Row({ label, value }: { label: string; value: React.ReactNode }) {
    return (
        <Box display="flex" justifyContent="space-between" gap={2} flexWrap="wrap">
            <Typography color="text.secondary">{label}</Typography>
            <Typography fontWeight={700} textAlign="right">
                {value}
            </Typography>
        </Box>
    );
}
