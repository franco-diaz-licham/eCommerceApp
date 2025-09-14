import { useClearBasketMutation, useFetchBasketQuery, useAddBasketItemMutation, useRemoveBasketItemMutation } from "../features/basket/services/basket.api";
import type { BasketItemResponse } from "../features/basket/types/basket.type";
import { skipToken } from "@reduxjs/toolkit/query";
import { useAppDispatch, useAppSelector } from "../app/store/store";
import { setBasketId } from "../features/basket/services/basketSlice";

export const useBasket = () => {
    const [addBasketItem, { isLoading: isAdding }] = useAddBasketItemMutation();
    const [removeItemMutation] = useRemoveBasketItemMutation();
    const [clearBasketMutation] = useClearBasketMutation();
    const dispatch = useAppDispatch();
    const currentId = useAppSelector((s) => s.basketSession.id); // get current session
    const { data: fetched, isLoading } = useFetchBasketQuery(currentId ?? skipToken); // get data from query
    const basket = fetched ?? null;

    // Use the correct key from your response
    const items: BasketItemResponse[] = basket?.basketItems ?? [];
    const subtotal = items.reduce((s, i) => s + i.lineTotal, 0);
    const deliveryFee = subtotal > 100 ? 0 : 5;
    const itemCount = items.reduce((sum, i) => sum + i.quantity, 0);

    let discount = 0;
    if (basket?.coupon) {
        discount = basket.coupon.amountOff ? basket.coupon.amountOff : Math.round(subtotal * ((basket.coupon.percentOff ?? 0) / 100) * 100) / 100;
    }
    const total = Math.round((subtotal - discount + deliveryFee) * 100) / 100;

    /** Adds new item to a basket. If basket does not exist, server will create a new basket. */
    const addItemEnsuringBasket = async (productId: number, quantity = 1) => {
        const payload = currentId ? { productId, quantity, basketId: currentId } : { productId, quantity };
        const updated = await addBasketItem(payload).unwrap();
        // update basketSlice.
        if (updated.id !== currentId) dispatch(setBasketId(updated.id));
        return updated;
    };

    /** Clears the entire basket. */
    const clearBasket = async () => await clearBasketMutation().unwrap();

    /** Get basketItem from current basket. */
    const getbasketItem = (productId: number) => {
        const item = items.find((x) => x.productId === productId);
        return item;
    };

    /** Remove item from busket. */
    const removeItemEnsuringBasket = async (productId: number, quantity = 1) => {
        const payload = currentId ? { productId, quantity, basketId: currentId } : { productId, quantity };
        const updated = await removeItemMutation(payload).unwrap();
        return updated;
    };

    return {
        basket,
        items,
        itemCount,
        subtotal,
        deliveryFee,
        discount,
        total,
        isAdding,
        isLoading,
        clearBasket,
        getbasketItem,
        removeItemEnsuringBasket,
        addItemEnsuringBasket,
    };
};
