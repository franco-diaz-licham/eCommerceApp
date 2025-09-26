import { Box, Button, Checkbox, FormControlLabel, Paper, Step, StepLabel, Stepper } from "@mui/material";
import { AddressElement, PaymentElement, useElements, useStripe } from "@stripe/react-stripe-js";
import { useState } from "react";
import Review from "./Review";
import type { ConfirmationToken, StripeAddressElementChangeEvent, StripePaymentElementChangeEvent } from "@stripe/stripe-js";
import { currencyFormat } from "../../../lib/utils";
import type { ConfirmationModel, PaymentSummaryModel, ShippingAddressModel } from "../models/checkout.type";


/** Internal address defaults model. */
export type AddressDefaults = {
    name?: string | null;
    address?: {
        line1?: string | null;
        line2?: string | null;
        city?: string | null;
        state?: string | null;
        postal_code?: string | null;
        country: string;
    };
};

/** Stepper defined steps. */
const steps = ["Address", "Payment", "Review"];

type CheckoutStepperProps = {
    /** Display-only */
    total: number;
    /** Address defaults for Stripe AddressElement */
    defaultAddress?: AddressDefaults;
    /** Called when user toggles “save address” on Address step */
    onSaveAddress?: (address: ShippingAddressModel) => Promise<void> | void;
    /** Called when user confirms the final step; parent builds OrderCreateDto */
    onConfirm: (args: ConfirmationModel) => Promise<void>;
    /** Guard for last step */
    clientSecret?: string;
};

export default function CheckoutStepper(props: CheckoutStepperProps) {
    const [activeStep, setActiveStep] = useState(0);
    const [saveAddressChecked, setSaveAddressChecked] = useState(false);
    const [addressComplete, setAddressComplete] = useState(false);
    const [paymentComplete, setPaymentComplete] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [confirmationToken, setConfirmationToken] = useState<ConfirmationToken | null>(null);
    const elements = useElements();
    const stripe = useStripe();
    const handleBack = () => setActiveStep((s) => Math.max(0, s - 1));
    const handleAddressChange = (event: StripeAddressElementChangeEvent) => setAddressComplete(event.complete);
    const handlePaymentChange = (event: StripePaymentElementChangeEvent) => setPaymentComplete(event.complete);

    /** Get Address information from stripe address element. */
    const getStripeAddress = async (): Promise<ShippingAddressModel | null> => {
        const addressElement = elements?.getElement("address");
        if (!addressElement) return null;
        const valueAddress = await addressElement.getValue();
        if (!valueAddress.value.name || !valueAddress.value.address) return null;
        return {
            name: valueAddress.value.name,
            line1: valueAddress.value.address.line1!,
            line2: valueAddress.value.address.line2 ?? undefined,
            city: valueAddress.value.address.city!,
            state: valueAddress.value.address.state ?? undefined,
            postalCode: valueAddress.value.address.postal_code!,
            country: valueAddress.value.address.country!,
        };
    };

    /** Get card payment information from stripe result.c */
    const getPaymentSummary = (): PaymentSummaryModel | null => {
        const pm = confirmationToken?.payment_method_preview?.card;
        if (!pm) return null;
        return {
            last4: pm.last4,
            brand: pm.brand,
            expMonth: pm.exp_month,
            expYear: pm.exp_year,
        };
    };

    /** Instantiates a token fro stripe payment confirmation.  */
    const createConfirmationToken = async () => {
        if (!elements || !stripe) return null;
        const result = await elements.submit();
        if (result.error) throw new Error(result.error.message);
        const res = await stripe.createConfirmationToken({ elements });
        if (res.error || !res.confirmationToken) throw new Error(res.error?.message || "Failed to create confirmation token");
        return res.confirmationToken;
    };

    /** Handles movement through the stepper. */
    const handleNext = async () => {
        try {
            // Save address
            if (activeStep === 0) {
                if (saveAddressChecked && props.onSaveAddress) {
                    const addr = await getStripeAddress();
                    if (addr) await props.onSaveAddress(addr);
                }
                setActiveStep(1);
                return;
            }

            // Create confirmation token
            if (activeStep === 1) {
                const token = await createConfirmationToken();
                if (!token) return;
                setConfirmationToken(token);
                setActiveStep(2);
                return;
            }

            // Submit and confirm payment.
            if (activeStep === 2) {
                if (!props.clientSecret) throw new Error("Missing client secret");
                const shippingAddress = await getStripeAddress();
                const paymentSummary = getPaymentSummary();
                if (!shippingAddress || !paymentSummary || !confirmationToken) throw new Error("Missing payment or address details");

                setSubmitting(true);
                await props.onConfirm({
                    shippingAddress,
                    paymentSummary,
                    confirmationToken: confirmationToken!,
                });
            }
        } catch (err: unknown) {
            if (activeStep > 0) setActiveStep((s) => s - 1);
            throw err;
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <Paper sx={{ p: 3, borderRadius: 3 }}>
            <Stepper activeStep={activeStep}>
                {steps.map((label, i) => (
                    <Step key={i}>
                        <StepLabel>{label}</StepLabel>
                    </Step>
                ))}
            </Stepper>

            <Box sx={{ mt: 2 }}>
                {/* Address */}
                <Box sx={{ display: activeStep === 0 ? "block" : "none" }}>
                    <AddressElement options={{ mode: "shipping", defaultValues: props.defaultAddress }} onChange={handleAddressChange} />
                    <FormControlLabel sx={{ display: "flex", justifyContent: "end" }} control={<Checkbox checked={saveAddressChecked} onChange={(e) => setSaveAddressChecked(e.target.checked)} />} label="Save as default address" />
                </Box>

                {/* Payment */}
                <Box sx={{ display: activeStep === 1 ? "block" : "none" }}>
                    <PaymentElement onChange={handlePaymentChange} options={{ wallets: { applePay: "never", googlePay: "never" } }} />
                </Box>

                {/* Review */}
                <Box sx={{ display: activeStep === 2 ? "block" : "none" }}>
                    <Review confirmationToken={confirmationToken} />
                </Box>
            </Box>

            <Box display="flex" paddingTop={2} justifyContent="space-between">
                <Button onClick={handleBack} disabled={activeStep === 0 || submitting}>
                    Back
                </Button>
                <Button onClick={handleNext} disabled={(activeStep === 0 && !addressComplete) || (activeStep === 1 && !paymentComplete) || submitting} loading={submitting}>
                    {activeStep === steps.length - 1 ? `Pay ${currencyFormat(props.total)}` : "Next"}
                </Button>
            </Box>
        </Paper>
    );
}
