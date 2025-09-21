import type { ConfirmationToken } from "@stripe/stripe-js";

export type ConfirmationModel = {
    shippingAddress: ShippingAddressModel;
    paymentSummary: PaymentSummaryModel;
    confirmationToken: ConfirmationToken;
};

export type ShippingAddressModel = {
    name: string;
    line1: string;
    line2?: string;
    city: string;
    state?: string;
    postalCode: string;
    country: string;
};

export type PaymentSummaryModel = {
    last4: string;
    brand: string;
    expMonth: number;
    expYear: number;
};
