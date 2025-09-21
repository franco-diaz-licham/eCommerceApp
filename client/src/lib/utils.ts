import type { PaymentSummaryDto, ShippingAddressDto } from "../features/order/models/order.type";
import type { ApiError, ApiValidationError } from "../types/api.types";


/** Formats currency. */
export function currencyFormat(amount: number) {
    return "$" + amount.toFixed(2);
}

export const formatAddressString = (address: ShippingAddressDto) => {
    return `${address?.line1}, ${address?.city}, ${address?.state}, 
            ${address?.postalCode}, ${address?.country}`;
};

export const formatPaymentString = (card: PaymentSummaryDto) => {
    return `${card?.brand?.toUpperCase()}, **** **** **** ${card?.last4}, 
            Exp: ${card?.expMonth}/${card?.expYear}`;
};

export function formatDate(date: string): string {
    return "dd MMM yyyy";
}

/** Receives an object and return the formData equivalent. */
export function createFormData<T extends object>(data: T) {
    const formData = new FormData();
    Object.entries(data as Record<string, unknown>).forEach(([key, value]) => {
        if (value == null) return;
        if (value instanceof Blob) formData.append(key, value);
        else if (Array.isArray(value)) value.forEach((v) => formData.append(key, String(v)));
        else if (typeof value === "object") formData.append(key, JSON.stringify(value));
        else formData.append(key, String(value));
    });
    return formData;
}

/** Handle validation errors and returns a message. */
export function getErrorMessage(error: unknown, fallback = "Operation failed") {
    if(error === null) return fallback;
    if (typeof error === "string") return error;
    if (error && typeof error === "object") {
        if ("validationErrors" in error) {
            const e = error as ApiValidationError;
            return e.validationErrors!.flat().join(" ") ?? fallback;
        } else if ("message" in error) {
            const e = error as ApiError;
            return e.message ?? e.details ?? fallback;
        }
    }

    return fallback;
}
