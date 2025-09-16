import { Grid, Typography } from "@mui/material";
import { loadStripe, type StripeElementsOptions } from "@stripe/stripe-js";
import { Elements } from "@stripe/react-stripe-js";
import { useEffect, useMemo, useRef } from "react";
import { useCreatePaymentIntentMutation } from "../services/checkout.api";
import { useAppSelector } from "../../../app/store/store";
import { useBasket } from "../../../hooks/useBasket";
import CheckoutStepper from "../components/CheckoutStepper";
import OrderSummary from "../../order/components/OrderSummary";

const stripePromise = loadStripe(import.meta.env.VITE_STRIPE_PK);

export default function CheckoutPage() {
    const { basket } = useBasket();
    const [createPaymentIntent, { isLoading }] = useCreatePaymentIntentMutation();
    const created = useRef(false);
    const { darkMode } = useAppSelector((state) => state.ui);

    useEffect(() => {
        if(!basket) return;
        if (!created.current) createPaymentIntent(basket.id);
        created.current = true;
    }, [basket, createPaymentIntent]);

    const options: StripeElementsOptions | undefined = useMemo(() => {
        if (!basket?.clientSecret) return undefined;
        return {
            clientSecret: basket.clientSecret,
            appearance: {
                labels: "floating",
                theme: darkMode ? "night" : "stripe",
            },
        };
    }, [basket?.clientSecret, darkMode]);

    return (
        <Grid container spacing={2}>
            <Grid size={8}>
                {!stripePromise || !options || isLoading ? (
                    <Typography variant="h6">Loading checkout...</Typography>
                ) : (
                    <Elements stripe={stripePromise} options={options}>
                        <CheckoutStepper />
                    </Elements>
                )}
            </Grid>
            <Grid size={4}>
                <OrderSummary />
            </Grid>
        </Grid>
    );
}
