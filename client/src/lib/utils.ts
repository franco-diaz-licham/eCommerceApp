import type { PaymentSummaryDto, ShippingAddressDto } from "../features/order/types/order.type";

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
