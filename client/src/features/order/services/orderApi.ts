import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import type { OrderCreate, OrderResponse } from "../types/order.type";

export const orderApi = createApi({
    reducerPath: "orderApi",
    baseQuery: baseQueryWithErrorHandling,
    tagTypes: ["Orders"],
    endpoints: (builder) => ({
        fetchOrders: builder.query<OrderResponse[], void>({
            query: () => "orders",
            providesTags: ["Orders"],
        }),
        fetchOrderDetailed: builder.query<OrderResponse, number>({
            query: (id) => ({
                url: `orders/${id}`,
            }),
        }),
        createOrder: builder.mutation<OrderCreate, OrderCreate>({
            query: (order) => ({
                url: "orders",
                method: "POST",
                body: order,
            }),
            onQueryStarted: async (_, { dispatch, queryFulfilled }) => {
                await queryFulfilled;
                dispatch(orderApi.util.invalidateTags(["Orders"]));
            },
        }),
    }),
});

export const { useFetchOrdersQuery, useFetchOrderDetailedQuery, useCreateOrderMutation } = orderApi;
