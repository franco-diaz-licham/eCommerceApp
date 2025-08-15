import type { Coupon } from "./Coupon.type";
import type { Item } from "./Item.type";

export type Basket = {
    basketId: string;
    items: Item[];
    clientSecret?: string;
    paymentIntentId?: string;
    coupon: Coupon | null;
};
