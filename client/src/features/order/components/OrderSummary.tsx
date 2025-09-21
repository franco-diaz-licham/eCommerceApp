import { Box, Typography, Divider, Button, TextField, Paper } from "@mui/material";
import { Link } from "react-router-dom";
import { type FieldValues, useForm } from "react-hook-form";
import { Delete } from "@mui/icons-material";
import { currencyFormat } from "../../../lib/utils";

export type OrderSummaryProps = {
    subtotal: number;
    discount: number;
    deliveryFee: number;
    total: number;
    inCheckout: boolean;
    couponCodeApplied?: string | null;
    onApplyCoupon?: (code: string) => Promise<void> | void;
    onRemoveCoupon?: () => Promise<void> | void;
};

export default function OrderSummary({ subtotal, discount, deliveryFee, total, inCheckout, couponCodeApplied, onApplyCoupon, onRemoveCoupon }: OrderSummaryProps) {
    const {
        register,
        handleSubmit,
        formState: { isSubmitting },
    } = useForm();

    const onSubmit = async (data: FieldValues) => {
        if (!onApplyCoupon) return;
        await onApplyCoupon(data.code as string);
    };

    return (
        <Box display="flex" flexDirection="column" alignItems="center" maxWidth="lg" mx="auto">
            <Paper sx={{ mb: 2, p: 3, width: "100%", borderRadius: 3 }}>
                <Typography variant="h6" component="p" fontWeight="bold">
                    Order summary
                </Typography>
                <Typography variant="body2" sx={{ fontStyle: "italic" }}>
                    Orders over $100 qualify for free delivery!
                </Typography>

                <Box mt={2}>
                    <Box display="flex" justifyContent="space-between" mb={1}>
                        <Typography color="textSecondary">Subtotal</Typography>
                        <Typography>{currencyFormat(subtotal)}</Typography>
                    </Box>
                    <Box display="flex" justifyContent="space-between" mb={1}>
                        <Typography color="textSecondary">Discount</Typography>
                        <Typography color="success">-{currencyFormat(discount)}</Typography>
                    </Box>
                    <Box display="flex" justifyContent="space-between" mb={1}>
                        <Typography color="textSecondary">Delivery fee</Typography>
                        <Typography>{currencyFormat(deliveryFee)}</Typography>
                    </Box>
                    <Divider sx={{ my: 2 }} />
                    <Box display="flex" justifyContent="space-between" mb={1}>
                        <Typography color="textSecondary">Total</Typography>
                        <Typography>{currencyFormat(total)}</Typography>
                    </Box>
                </Box>

                <Box mt={2}>
                    {!inCheckout && (
                        <Button component={Link} to="/checkout" variant="contained" color="primary" fullWidth sx={{ mb: 1 }}>
                            Checkout
                        </Button>
                    )}
                    <Button component={Link} to="/products" fullWidth>
                        Continue Shopping
                    </Button>
                </Box>
            </Paper>

            {/* Coupon section */}
            {inCheckout && (
                <Paper sx={{ width: "100%", borderRadius: 3, p: 3 }}>
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <Typography variant="subtitle1" component="label">
                            Do you have a voucher code?
                        </Typography>

                        {couponCodeApplied && (
                            <Box display="flex" justifyContent="space-between" alignItems="center">
                                <Typography fontWeight="bold" variant="body2">
                                    {couponCodeApplied} applied
                                </Typography>
                                <Button onClick={onRemoveCoupon} /* loading prop if your Button supports it */>
                                    <Delete color="error" />
                                </Button>
                            </Box>
                        )}

                        <TextField label="Voucher code" variant="outlined" fullWidth disabled={!!couponCodeApplied} {...register("code", { required: "Voucher code missing" })} sx={{ my: 2 }} />

                        <Button
                            type="submit"
                            variant="contained"
                            color="primary"
                            fullWidth
                            disabled={!!couponCodeApplied}
                            // loading={isSubmitting} // if you have a loading Button
                        >
                            Apply code
                        </Button>
                    </form>
                </Paper>
            )}
        </Box>
    );
}
