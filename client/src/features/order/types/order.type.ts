import type { OrderItem, PaymentSummary, ShippingAddress } from "../../basket/types/order.type";

export interface OrderResponse {
    id: number;
    buyerEmail: string;
    shippingAddress: ShippingAddress;
    orderDate: string;
    orderItems: OrderItem[];
    subtotal: number;
    deliveryFee: number;
    discount: number;
    total: number;
    orderStatus: string;
    paymentSummary: PaymentSummary;
}

export interface OrderCreate {
    shippingAddress: ShippingAddress;
    paymentSummary: PaymentSummary;
}
