import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import type { BasketCouponDto, BasketItemDto, BasketResponse } from "../types/basket.type";

/** Base url resource endpoint. */
const baseUrl: string = "basket";

const getActiveBasket = () => `${baseUrl}/active`;

const createBasket = () => {
    return {
        url: baseUrl,
        method: "POST",
    };
};

const addBasketItem = (data: BasketItemDto) => {
    return {
        url: `${baseUrl}/add-item`,
        method: "POST",
        body: data,
    };
};

const removeBasketItem = (data: BasketItemDto) => {
    return {
        url: `${baseUrl}/remove-item`,
        method: "DELETE",
        body: data,
    };
};

const addCoupon = (data: BasketCouponDto) => {
    return {
        url: `basket/${data}`,
        method: "POST",
        body: data,
    };
};

const removeCoupon = (data: BasketCouponDto) => {
    return {
        url: "basket/remove-coupon",
        method: "DELETE",
        body: data,
    };
};

export const basketApi = createApi({
    reducerPath: "basketApi",
    baseQuery: baseQueryWithErrorHandling,
    tagTypes: ["Basket"],
    endpoints: (builder) => ({
        createBasket: builder.mutation<BasketResponse, void>({
            query: () => createBasket(),
        }),
        fetchBasket: builder.query<BasketResponse, void>({
            query: () => getActiveBasket(),
            providesTags: ["Basket"],
        }),
        addBasketItem: builder.mutation<BasketResponse, BasketItemDto>({
            query: (data) => addBasketItem(data),
        }),
        removeBasketItem: builder.mutation<void, BasketItemDto>({
            query: (data) => removeBasketItem(data),
        }),
        clearBasket: builder.mutation<void, void>({
            queryFn: () => ({ data: undefined }),
            onQueryStarted: async (_, { dispatch, getState }) => {
                // Read current cached basket
                const sel = basketApi.endpoints.fetchBasket.select();
                const basket = sel(getState()).data;

                if (!basket || !basket.items?.length) return;

                // Kick off remove calls in parallel (one per item, removing full quantity)
                const removals = basket.items.map((item) => dispatch(basketApi.endpoints.removeBasketItem.initiate({ productId: item.productId, quantity: item.quantity, basketId: item.id }, { track: false })).unwrap());

                // Wait for all to settle (don’t throw if one fails)
                await Promise.allSettled(removals);

                // Finally, refresh basket from server to ensure truth
                dispatch(basketApi.util.invalidateTags(["Basket"]));
            },
        }),
        addCoupon: builder.mutation<BasketResponse, BasketCouponDto>({
            query: (data) => addCoupon(data),
        }),
        removeCoupon: builder.mutation<void, BasketCouponDto>({
            query: (data) => removeCoupon(data),
        }),
    }),
});

export const { useFetchBasketQuery, useAddBasketItemMutation, useAddCouponMutation, useRemoveCouponMutation, useRemoveBasketItemMutation, useClearBasketMutation } = basketApi;
