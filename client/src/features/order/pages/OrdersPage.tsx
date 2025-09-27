import { Box, Button, Typography } from "@mui/material";
import { Link, useNavigate } from "react-router-dom";
import { useFetchOrdersQuery } from "../api/orderApi";
import { useAppSelector } from "../../../app/store/store";
import Header from "../../../components/ui/Header";
import OrdersTable from "../components/OrdersTable";
import WarningAmberIcon from '@mui/icons-material/WarningAmber';

export default function OrdersPage() {
    const params = useAppSelector((state) => state.orders);
    const { data: orders, isLoading } = useFetchOrdersQuery(params);
    const navigate = useNavigate();

    if (isLoading) return;
    if (!orders || orders.response.length === 0) {
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
                    There are no orders
                </Typography>
                <Button component={Link} to="/products" size="large" variant="contained">
                    Go back to shop
                </Button>
            </Box>
        );
    }

    return (
        <Box sx={{ pt: 4 }}>
            {/* Header */}
            <Header title="My Orders" />

            {/* Table */}
            <OrdersTable orders={orders} OnOrderClicked={(id) => navigate(`/orders/${id}`)} />
        </Box>
    );
}
