import type { CouponResponse } from "./coupon.type";

export type BasketItemResponse = {
    id: number;
    basketId: number;
    productId: number;
    name: string;
    price: number;
    pictureUrl: string;
    brand: string;
    type: string;
    quantity: number;
};

export type BasketResponse = {
    id: number;
    items: BasketItemResponse[];
    clientSecret?: string;
    paymentIntentId?: string;
    subtotal: number;
    couponId: number | null;
    coupon: CouponResponse | null;
};

export type BasketItemDto = {
    basketId: number;
    productId: number;
    quantity: number;
};

export type BasketClearDto = {
    id: string;
    items: number[];
};

export type BasketCouponDto = {
    basketId: number;
    couponId: number;
};
