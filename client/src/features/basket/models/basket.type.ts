import type { CouponResponse } from "./coupon.type";

/** Basket item Dto response. */
export type BasketItemResponse = {
    basketId: number;
    productId: number;
    name: string;
    unitPrice: number;
    quantity: number;
    publicUrl: string;
    lineTotal: number;
};

/** Basket Dto Response. */
export type BasketResponse = {
    id: number;
    basketItems: BasketItemResponse[];
    clientSecret?: string;
    paymentIntentId?: string;
    subtotal: number;
    couponId: number | null;
    coupon: CouponResponse | null;
};

/** Dto for items added to the basket. */
export type BasketItemDto = {
    basketId?: number;
    productId: number;
    quantity: number;
};

/** Dto for clearing all items from a basket. */
export type BasketClearDto = {
    id: string;
    basketItems: number[];
};

/** Dto for coupon added to a basket. */
export type BasketCouponDto = {
    basketId: number;
    promotionCode: string;
};
