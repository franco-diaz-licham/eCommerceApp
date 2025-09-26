import type { ConfirmationToken } from "@stripe/stripe-js";

/** Payment intent dto response. */
export interface PaymentIntentResponse {
    clientSecret: string;
    paymentIntent: string;
}

/** Checkout confirmation checkout stepper model. */
export type ConfirmationModel = {
    shippingAddress: ShippingAddressModel;
    paymentSummary: PaymentSummaryModel;
    confirmationToken: ConfirmationToken;
};

/** Shipping address checkout stepper model */
export type ShippingAddressModel = {
    name: string;
    line1: string;
    line2?: string;
    city: string;
    state?: string;
    postalCode: string;
    country: string;
};

/** Payment summary checkout stepper model. */
export type PaymentSummaryModel = {
    last4: string;
    brand: string;
    expMonth: number;
    expYear: number;
};
