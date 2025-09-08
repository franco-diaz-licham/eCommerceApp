import type { FieldValues, Path, UseFormSetError } from "react-hook-form";
import type { PaymentSummary, ShippingAddress } from "../features/basket/types/order.type";

/** Formats currency. */
export function currencyFormat(amount: number) {
    return "$" + amount.toFixed(2);
}

/** Sets query filters for api query params.  */
export function filterEmptyValues(values: object) {
    return Object.fromEntries(Object.entries(values).filter(([, value]) => value !== "" && value !== null && value !== undefined && value.length !== 0));
}

export const formatAddressString = (address: ShippingAddress) => {
    return `${address?.name}, ${address?.line1}, ${address?.city}, ${address?.state}, 
            ${address?.postal_code}, ${address?.country}`;
};

export const formatPaymentString = (card: PaymentSummary) => {
    return `${card?.brand?.toUpperCase()}, **** **** **** ${card?.last4}, 
            Exp: ${card?.exp_month}/${card?.exp_year}`;
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
