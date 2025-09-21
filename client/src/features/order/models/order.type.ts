import type { Pagination } from "../../../types/pagination.type";

export interface OrderResponse {
    id: number;
    userId: string;
    shippingAddress: ShippingAddressDto;
    paymentSummary: PaymentSummaryDto;
    orderDate: string;
    orderItems: OrderItem[];
    subtotal: number;
    deliveryFee: number;
    discount: number;
    total: number;
    orderStatus: string;
}

export interface OrderCreateDto {
    basketId: number;
    shippingAddress: ShippingAddressDto;
    paymentSummary: PaymentSummaryDto;
}

export interface ShippingAddressDto {
    recipientName: string;
    line1: string;
    line2?: string | null;
    city: string;
    state: string;
    postalCode: string;
    country: string;
}

export interface PaymentSummaryDto {
    last4: number | string;
    brand: string;
    expMonth: number;
    expYear: number;
}

export interface OrderItem {
    productId: number;
    name: string;
    pictureUrl: string;
    price: number;
    quantity: number;
}

/** Paginated orders response */
export interface PaginatedOrdersData {
    response: OrderResponse[];
    pagination: Pagination;
}