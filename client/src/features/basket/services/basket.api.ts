import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import type { BasketCouponDto, BasketItemDto, BasketResponse } from "../types/basket.type";
import type { ApiSingleResponse } from "../../../types/api.types";

/** Base url resource endpoint. */
const baseUrl: string = "basket";

const transformPaginatedSingleResponse = (r: ApiSingleResponse<BasketResponse>) => (r && typeof r === "object" && "data" in r ? (r as { data: BasketResponse }).data : (r as BasketResponse));

// Keep a stable "active" endpoint so components don't need an id
const getBasket = (id: number) => `${baseUrl}/${id}`;

const createBasket = () => ({
    url: baseUrl,
    method: "POST",
});

const addBasketItem = (data: BasketItemDto) => ({
    url: `${baseUrl}/add-item`,
    method: "POST",
    body: data,
});

const removeBasketItem = (data: BasketItemDto) => ({
    url: `${baseUrl}/remove-item`,
    method: "DELETE",
    body: data,
});

const addCoupon = (data: BasketCouponDto) => ({
    url: `${baseUrl}/add-coupon`,
    method: "POST",
    body: data,
});

const removeCoupon = (data: BasketCouponDto) => ({
    url: `${baseUrl}/remove-coupon`,
    method: "DELETE",
    body: data,
});

export const basketApi = createApi({
    reducerPath: "basketApi",
    baseQuery: baseQueryWithErrorHandling,
    tagTypes: ["Basket"],
    endpoints: (builder) => ({
        createBasket: builder.mutation<BasketResponse, void>({
            query: () => createBasket(),
            invalidatesTags: ["Basket"],
        }),
        fetchBasket: builder.query<BasketResponse, number>({
            query: (data) => getBasket(data),
            transformResponse: transformPaginatedSingleResponse,
            providesTags: (id) => [{ type: "Basket", id: Number(id) }],
        }),
        addBasketItem: builder.mutation<BasketResponse, BasketItemDto>({
            query: (data) => addBasketItem(data),
            transformResponse: transformPaginatedSingleResponse,
            onQueryStarted: async (_, { dispatch, queryFulfilled }) => {
                try {
                    const { data: updated } = await queryFulfilled;
                    dispatch(basketApi.util.upsertQueryData("fetchBasket", updated.id, updated));
                } catch {
                    // ignore; error handling is in baseQueryWithErrorHandling
                }
            },
        }),
        removeBasketItem: builder.mutation<void, BasketItemDto>({
            query: (data) => removeBasketItem(data),
            invalidatesTags: ["Basket"],
        }),
        clearBasket: builder.mutation<void, void>({
            queryFn: () => ({ data: undefined }),
            async onQueryStarted(_, { dispatch, getState }) {
                const basketId = localStorage.getItem("basketId");
                if(!basketId) return;
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
        addCoupon: builder.mutation<BasketResponse, BasketCouponDto>({
            query: (data) => addCoupon(data),
            invalidatesTags: ["Basket"],
        }),
        removeCoupon: builder.mutation<void, BasketCouponDto>({
            query: (data) => removeCoupon(data),
            invalidatesTags: ["Basket"],
        }),
    }),
});

export const { useFetchBasketQuery, useAddBasketItemMutation, useAddCouponMutation, useRemoveCouponMutation, useRemoveBasketItemMutation, useClearBasketMutation, useCreateBasketMutation } = basketApi;
