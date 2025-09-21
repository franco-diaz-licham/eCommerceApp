import { Box, Grid } from "@mui/material";
import { loadStripe, type StripeElementsOptions } from "@stripe/stripe-js";
import { Elements } from "@stripe/react-stripe-js";
import { useEffect, useMemo, useRef } from "react";
import { useAppSelector } from "../../../app/store/store";
import { useBasket } from "../../../hooks/useBasket";
import OrderSummary from "../../order/components/OrderSummary";
import { useCreateUserAddressMutation, useFetchAddressQuery, useUpdateUserAddressMutation, useUserInfoQuery } from "../../authentication/api/account.api";
import { toast } from "react-toastify";
import { useNavigate, useLocation } from "react-router-dom";
import { useAddCouponMutation, useRemoveCouponMutation } from "../../basket/api/basket.api";
import { useCreatePaymentIntentMutation } from "../api/checkout.api";
import { useCreateOrderMutation } from "../../order/api/orderApi";
import type { OrderCreateDto } from "../../order/models/order.type";
import { mapToAddressCreateDto, mapToShippingAddressDto } from "../api/checkoutMapper";
import type { ConfirmationModel, ShippingAddressModel } from "../models/ui";
import CheckoutStepper, { type AddressDefaults } from "../components/CheckoutStepper";
import Header from "../../../components/ui/Header";
import { getErrorMessage } from "../../../lib/utils";

const stripePromise = loadStripe(import.meta.env.VITE_STRIPE_PK);

export default function CheckoutPage() {
    const navigate = useNavigate();
    const location = useLocation();
    const inCheckout = location.pathname.includes("checkout");

    const { darkMode } = useAppSelector((s) => s.ui);
    const { data: user } = useUserInfoQuery();
    const { basket, subtotal, deliveryFee, discount, total, deleteBasket } = useBasket();

    const { data: addrData, isLoading: addrLoading } = useFetchAddressQuery();
    const defaultAddress: AddressDefaults | undefined = addrData ? { name: "", address: addrData } : undefined;

    const [createPaymentIntent, { isLoading: creatingPI }] = useCreatePaymentIntentMutation();
    const [createOrder] = useCreateOrderMutation();
    const [updateAddress] = useUpdateUserAddressMutation();
    const [createAddress] = useCreateUserAddressMutation();
    const [addCoupon] = useAddCouponMutation();
    const [removeCoupon] = useRemoveCouponMutation();

    const created = useRef(false);

    useEffect(() => {
        if (!basket) return;
        if (!created.current) createPaymentIntent(basket.id);
        created.current = true;
    }, [basket, createPaymentIntent]);

    const options: StripeElementsOptions | undefined = useMemo(() => {
        if (!basket?.clientSecret) return undefined;
        return {
            clientSecret: basket.clientSecret,
            appearance: { labels: "floating", theme: darkMode ? "night" : "stripe" },
        };
    }, [basket?.clientSecret, darkMode]);

    /** Handle saving users default address. */
    const handleSaveAddress = async (model: ShippingAddressModel) => {
        const address = mapToAddressCreateDto(model);
        try {
            if (!user?.AddressId) await createAddress(address).unwrap();
            else await updateAddress({ id: user.AddressId, ...address }).unwrap();
        } catch (e: unknown) {
            toast.error(getErrorMessage(e, "Failed to save address"));
        }
    };

    /** Parent assembles API DTO and confirms payment */
    const handleConfirm = async ({ shippingAddress, paymentSummary, confirmationToken }: ConfirmationModel) => {
        try {
            if (!basket?.clientSecret || !basket?.id) throw new Error("Unable to process payment (missing basket)");
            const shipAddress = mapToShippingAddressDto(shippingAddress);
            const orderInput: OrderCreateDto = {
                basketId: basket.id,
                shippingAddress: shipAddress,
                paymentSummary,
            };

            const orderResult = await createOrder(orderInput).unwrap();

            const stripe = await stripePromise;
            if (!stripe) throw new Error("Stripe not initialized");

            const paymentResult = await stripe.confirmPayment({
                clientSecret: basket.clientSecret,
                redirect: "if_required",
                confirmParams: { confirmation_token: confirmationToken.id },
            });

            if (paymentResult?.paymentIntent?.status === "succeeded") {
                await deleteBasket();
                navigate("/checkout/success", { state: orderResult });
            } else if (paymentResult?.error) {
                throw new Error(paymentResult.error.message);
            } else {
                throw new Error("Something went wrong during payment");
            }
        } catch (e: unknown) {
            toast.error(getErrorMessage(e, "Payment failed"));
            throw e;
        }
    };

    if (!stripePromise || !options || creatingPI || addrLoading) return;

    return (
        <Box sx={{ pt: 4 }}>
            {/* Header */}
            <Header title="Checkout" children={undefined} />

            {/* Content */}
            <Grid container spacing={2}>
                <Grid size={8}>
                    <Elements stripe={stripePromise} options={options}>
                        <CheckoutStepper total={total} defaultAddress={defaultAddress} onSaveAddress={handleSaveAddress} onConfirm={handleConfirm} clientSecret={basket?.clientSecret} />
                    </Elements>
                </Grid>

                <Grid size={4}>
                    <OrderSummary
                        subtotal={subtotal}
                        discount={discount}
                        deliveryFee={deliveryFee}
                        total={total}
                        inCheckout={inCheckout}
                        couponCodeApplied={basket?.coupon?.promotionCode ?? null}
                        onApplyCoupon={async (code) => {
                            try {
                                await addCoupon({ basketId: basket!.id, promotionCode: code }).unwrap();
                            } catch (e: unknown) {
                                toast.error(getErrorMessage(e, "Failed to apply code"));
                            }
                        }}
                        onRemoveCoupon={async () => {
                            if (!basket?.id || !basket?.couponId) return;
                            try {
                                await removeCoupon({
                                    basketId: basket.id,
                                    promotionCode: basket.coupon?.promotionCode ?? "",
                                }).unwrap();
                            } catch (e: unknown) {
                                toast.error(getErrorMessage(e, "Failed to remove code"));
                            }
                        }}
                    />
                </Grid>
            </Grid>
        </Box>
    );
}
