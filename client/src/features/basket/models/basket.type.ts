import type { CouponResponse } from "./coupon.type";

export type BasketItemResponse = {
    basketId: number;
    productId: number;
    name: string;
    unitPrice: number;
    quantity: number;
    publicUrl: string;
    lineTotal: number;
};

export type BasketResponse = {
    id: number;
    basketItems: BasketItemResponse[];
    clientSecret?: string;
    paymentIntentId?: string;
    subtotal: number;
    couponId: number | null;
    coupon: CouponResponse | null;
};

export type BasketItemDto = {
    basketId?: number;
    productId: number;
    quantity: number;
};

export type BasketClearDto = {
    id: string;
    basketItems: number[];
};

export type BasketCouponDto = {
    basketId: number;
    promotionCode: string;
};
