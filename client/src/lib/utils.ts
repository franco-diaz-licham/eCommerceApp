import type { FetchBaseQueryMeta } from "@reduxjs/toolkit/query";
import type { ApiError, ApiResponse, ApiSingleResponse, ApiValidationError } from "../types/api.types";

/** Formats currency. */
export function currencyFormat(amount: number) {
    return "$" + amount.toFixed(2);
}

/** Format the display of address information. */
export const formatAddressString = (line1: string | null, city: string | null, state: string | null, code: string | null, country: string | null, name?: string | null) => {
    const address = `${line1}, ${city}, ${state}, ${code}, ${country}`;
    if (name) return `${name}, ` + address;
    return address;
};

/** Format the display of payment information. */
export const formatPaymentString = (brand: string, last4: string, expMonth: number, expYear: number) => {
    return `${brand.toUpperCase()}, **** **** **** ${last4}, Exp: ${expMonth}/${expYear}`;
};

/** format date to dd-MM-yy. */
export function formatDate(date: string): string {
    const newDate = new Date(date);
    const year = newDate.getFullYear();
    const month = (newDate.getMonth() + 1).toString().padStart(2, "0");
    const day = newDate.getDate().toString().padStart(2, "0");
    return `${day}/${month}/${year}`;
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
    if (error === null) return fallback;
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

/** Response list handler. */
export function transformPaginatedResponse<T>(response: ApiResponse<T>, meta: FetchBaseQueryMeta) {
    const paginationHeader = meta?.response?.headers.get("Pagination");
    const pagination = paginationHeader ? JSON.parse(paginationHeader) : null;
    const data = response.data;
    return { response: data, pagination };
}

/** Handles HTTP request reponses as well as upsertion from other reducers. */
export function transformSingleResponse<T>(response: ApiSingleResponse<T>) {
    if (response && typeof response === "object" && "data" in response) return response.data;
    else return response;
}
