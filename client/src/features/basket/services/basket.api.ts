import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import type { BasketCouponDto, BasketItemDto, BasketResponse } from "../types/basket.type";
import type { ApiSingleResponse } from "../../../types/api.types";
import { nothing } from "immer";

/** Base url resource endpoint. */
const baseUrl: string = "basket";

/** Hanldes HTTP request reponses as well as upsertion from other reducers. */
const transformSingleResponse = (response: ApiSingleResponse<BasketResponse>) => (response && typeof response === "object" && "data" in response ? (response as { data: BasketResponse }).data : (response as BasketResponse));

export const basketApi = createApi({
    reducerPath: "basketApi",
    baseQuery: baseQueryWithErrorHandling,
    tagTypes: ["Basket"],
    endpoints: (builder) => ({
        // CREATE and empty basket.
        createBasket: builder.mutation<BasketResponse, void>({
            query: () => ({
                url: baseUrl,
                method: "POST",
            }),
            invalidatesTags: ["Basket"],
        }),
        // GET basket by id.
        fetchBasket: builder.query<BasketResponse, number>({
            query: (id) => `${baseUrl}/${id}`,
            transformResponse: transformSingleResponse,
            providesTags: (id) => [{ type: "Basket", id: Number(id) }],
        }),
        // DELETE basket cache.
        deleteBasket: builder.mutation<void, number>({
            queryFn: () => ({ data: undefined }),
            // Invalidate so others refetch
            invalidatesTags: (result, error, id) => [{ type: "Basket", id }],
            onQueryStarted: async (id, { dispatch, getState, queryFulfilled }) => {
                // Read current basketId from localStorage
                const basketIdStr = localStorage.getItem("basketId");
                const basketId = basketIdStr ? Number(basketIdStr) : id;
                
                // Remove cached entry
                const cached = basketApi.endpoints.fetchBasket.select(basketId)(getState())?.data;
                let patch: { undo?: () => void } | undefined;
                if (cached) patch = dispatch(basketApi.util.updateQueryData("fetchBasket", basketId, () => nothing as unknown as BasketResponse));
                localStorage.removeItem("basketId");

                try {
                    await queryFulfilled;
                } catch {
                    patch?.undo?.();
                    localStorage.setItem("basketId", String(basketId));
                }
            },
        }),
        // CREATE item and add to basket.
        addBasketItem: builder.mutation<BasketResponse, BasketItemDto>({
            query: (data) => ({
                url: `${baseUrl}/add-item`,
                method: "POST",
                body: data,
            }),
            transformResponse: transformSingleResponse,
            onQueryStarted: async (_, { dispatch, queryFulfilled }) => {
                try {
                    const { data: updated } = await queryFulfilled;
                    dispatch(basketApi.util.upsertQueryData("fetchBasket", updated.id, updated));
                } catch {
                    // ignore; error handling is in baseQueryWithErrorHandling
                }
            },
        }),
        // DELETE basket item from basket.
        removeBasketItem: builder.mutation<void, BasketItemDto>({
            query: (data) => ({
                url: `${baseUrl}/remove-item`,
                method: "DELETE",
                body: data,
            }),
            invalidatesTags: ["Basket"],
        }),
        // DELETE all items in the basket.
        clearBasket: builder.mutation<void, void>({
            queryFn: () => ({ data: undefined }),
            onQueryStarted: async (_, { dispatch, getState }) => {
                // get basketId from local storage
                const basketId = localStorage.getItem("basketId");
                if (!basketId) return;
                // get basket state
                const sel = basketApi.endpoints.fetchBasket.select(Number(basketId));
                const basket = sel(getState()).data;
                if (!basket || !basket.basketItems?.length) return;
                // Remove each item fully (use the basket's id, not item.id)
                const removals = basket.basketItems.map((item) => dispatch(basketApi.endpoints.removeBasketItem.initiate({ productId: item.productId, quantity: item.quantity, basketId: basket.id }, { track: false })).unwrap());
                await Promise.allSettled(removals);
                // Ensure re-fetch for source of truth
                dispatch(basketApi.util.invalidateTags(["Basket"]));
            },
        }),
        // CREATE coupon and add to basket for discount.
        addCoupon: builder.mutation<BasketResponse, BasketCouponDto>({
            query: (data) => ({
                url: `${baseUrl}/add-coupon`,
                method: "POST",
                body: data,
            }),
            invalidatesTags: ["Basket"],
        }),
        // DELETE coupon from basket.
        removeCoupon: builder.mutation<void, BasketCouponDto>({
            query: (data) => ({
                url: `${baseUrl}/remove-coupon`,
                method: "DELETE",
                body: data,
            }),
            invalidatesTags: ["Basket"],
        }),
    }),
});

export const { useFetchBasketQuery, useAddBasketItemMutation, useAddCouponMutation, useRemoveCouponMutation, useRemoveBasketItemMutation, useClearBasketMutation, useCreateBasketMutation, useDeleteBasketMutation } = basketApi;
