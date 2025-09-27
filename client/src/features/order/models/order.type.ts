import type { OrderStatusResponse } from "../../../entities/orderStatus/orderStatus.type";
import type { Pagination } from "../../../types/pagination.type";

/** Main response model for orders. */
export interface OrderResponse {
    id: number;
    shippingAddress: ShippingAddressDto;
    paymentSummary: PaymentSummaryDto;
    orderDate: string;
    orderItems: OrderItem[];
    subtotal: number;
    deliveryFee: number;
    discount: number;
    total: number;
    orderStatus: OrderStatusResponse;
}

/** Order dto for creation. */
export interface OrderCreateDto {
    basketId: number;
    shippingAddress: ShippingAddressDto;
    paymentSummary: PaymentSummaryDto;
}

/** Shipping address dto. */
export interface ShippingAddressDto {
    recipientName: string;
    line1: string;
    line2?: string | null;
    city: string;
    state: string;
    postalCode: string;
    country: string;
}

/** Payment summary dto. */
export interface PaymentSummaryDto {
    last4: number | string;
    brand: string;
    expMonth: number;
    expYear: number;
}

/** Order item model. */
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