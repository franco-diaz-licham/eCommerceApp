import { Box, Button, Checkbox, FormControlLabel, Paper, Step, StepLabel, Stepper } from "@mui/material";
import { AddressElement, PaymentElement, useElements, useStripe } from "@stripe/react-stripe-js";
import { useState } from "react";
import Review from "./Review";
import type { ConfirmationToken, StripeAddressElementChangeEvent, StripePaymentElementChangeEvent } from "@stripe/stripe-js";
import { currencyFormat } from "../../../lib/utils";
import type { ConfirmationModel, PaymentSummaryModel, ShippingAddressModel } from "../models/ui";

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

// ------------------------------------------------

const steps = ["Address", "Payment", "Review"];

type Props = {
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

export default function CheckoutStepper({ total, defaultAddress, onSaveAddress, onConfirm, clientSecret }: Props) {
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

    const getStripeAddress = async (): Promise<ShippingAddressModel | null> => {
        const addressElement = elements?.getElement("address");
        if (!addressElement) return null;
        const {
            value: { name, address },
        } = await addressElement.getValue();
        if (!name || !address) return null;
        return {
            name: name,
            line1: address.line1!,
            line2: address.line2 ?? undefined,
            city: address.city!,
            state: address.state ?? undefined,
            postalCode: address.postal_code!,
            country: address.country!,
        };
    };

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

    const createConfirmationToken = async () => {
        if (!elements || !stripe) return null;
        const result = await elements.submit();
        if (result.error) throw new Error(result.error.message);
        const res = await stripe.createConfirmationToken({ elements });
        if (res.error || !res.confirmationToken) throw new Error(res.error?.message || "Failed to create confirmation token");
        return res.confirmationToken;
    };

    const handleNext = async () => {
        try {
            if (activeStep === 0) {
                if (saveAddressChecked && onSaveAddress) {
                    const addr = await getStripeAddress();
                    if (addr) await onSaveAddress(addr);
                }
                setActiveStep(1);
                return;
            }

            if (activeStep === 1) {
                const token = await createConfirmationToken();
                if (!token) return;
                setConfirmationToken(token);
                setActiveStep(2);
                return;
            }

            if (activeStep === 2) {
                if (!clientSecret) throw new Error("Missing client secret");
                const shippingAddress = await getStripeAddress();
                const paymentSummary = getPaymentSummary();
                if (!shippingAddress || !paymentSummary || !confirmationToken) throw new Error("Missing payment or address details");

                setSubmitting(true);
                await onConfirm({
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
                    <AddressElement options={{ mode: "shipping", defaultValues: defaultAddress }} onChange={handleAddressChange} />
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
                    {activeStep === steps.length - 1 ? `Pay ${currencyFormat(total)}` : "Next"}
                </Button>
            </Box>
        </Paper>
    );
}
