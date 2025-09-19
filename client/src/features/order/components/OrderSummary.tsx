import { Box, Typography, Divider, Button, TextField, Paper } from "@mui/material";
import { Link, useLocation } from "react-router-dom";
import { type FieldValues, useForm } from "react-hook-form";
import { Delete } from "@mui/icons-material";
import { useAddCouponMutation, useRemoveCouponMutation } from "../../basket/services/basket.api";
import { currencyFormat } from "../../../lib/utils";
import { useBasket } from "../../../hooks/useBasket";

export default function OrderSummary() {
    const {subtotal, deliveryFee, discount, basket, total} = useBasket();
    const location = useLocation();
    const {register, handleSubmit, formState: {isSubmitting}} = useForm();
    const [addCoupon] = useAddCouponMutation();
    const [removeCoupon, {isLoading}] = useRemoveCouponMutation();

    const onSubmit = async (data: FieldValues) => {
            await addCoupon(data.code);
    } 

    return (
        <Box display="flex" flexDirection="column" alignItems="center" maxWidth="lg" mx="auto">
            <Paper sx={{ mb: 2, p: 3, width: '100%', borderRadius: 3 }}>

                <Typography variant="h6" component="p" fontWeight="bold">
                    Order summary
                </Typography>
                <Typography variant="body2" sx={{fontStyle: 'italic'}}>
                    Orders over $100 qualify for free delivery!
                </Typography>
                <Box mt={2}>
                    <Box display="flex" justifyContent="space-between" mb={1}>
                        <Typography color="textSecondary">Subtotal</Typography>
                        <Typography>
                            {currencyFormat(subtotal)}
                        </Typography>
                    </Box>
                    <Box display="flex" justifyContent="space-between" mb={1}>
                        <Typography color="textSecondary">Discount</Typography>
                        <Typography color="success">
                            -{currencyFormat(discount)}
                        </Typography>
                    </Box>
                    <Box display="flex" justifyContent="space-between" mb={1}>
                        <Typography color="textSecondary">Delivery fee</Typography>
                        <Typography>
                            {currencyFormat(deliveryFee)}
                        </Typography>
                    </Box>
                    <Divider sx={{ my: 2 }} />
                    <Box display="flex" justifyContent="space-between" mb={1}>
                        <Typography color="textSecondary">Total</Typography>
                        <Typography>
                            {currencyFormat(total)}
                        </Typography>
                    </Box>
                </Box>

                <Box mt={2}>
                    {!location.pathname.includes('checkout') &&
                    <Button
                        component={Link}
                        to='/checkout'
                        variant="contained"
                        color="primary"
                        fullWidth
                        sx={{ mb: 1 }}
                    >
                        Checkout
                    </Button>}
                    <Button
                        component={Link}
                        to='/products'
                        fullWidth
                    >
                        Continue Shopping
                    </Button>
                </Box>
            </Paper>

            {/* Coupon Code Section */}
            {location.pathname.includes('checkout') &&
            <Paper sx={{ width: '100%', borderRadius: 3, p: 3 }}>

                <form onSubmit={handleSubmit(onSubmit)}>
                    <Typography variant="subtitle1" component="label">
                        Do you have a voucher code?
                    </Typography>

                    {basket?.coupon &&
                    <Box display='flex' justifyContent='space-between' alignItems='center'>
                        <Typography fontWeight='bold' variant='body2'>{basket.coupon.promotionCode} applied</Typography>
                        <Button loading={isLoading} onClick={() => removeCoupon({basketId: basket.id, couponId: basket.couponId!})}>
                            <Delete color="error" />
                        </Button>
                    </Box>}

                    
                    <TextField
                        label="Voucher code"
                        variant="outlined"
                        fullWidth
                        disabled={!!basket?.coupon}
                        {...register('code', {required: 'Voucher code missing'})}
                        sx={{ my: 2 }}
                    />

                    <Button
                        loading={isSubmitting}
                        type="submit"
                        variant="contained"
                        color="primary"
                        fullWidth
                        disabled={!!basket?.coupon}
                    >
                        Apply code
                    </Button>
                </form>
            </Paper>}
        </Box>
    )
}