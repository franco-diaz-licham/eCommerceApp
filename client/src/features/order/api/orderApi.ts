import { createApi, type FetchBaseQueryMeta } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import type { OrderCreateDto, OrderResponse, PaginatedOrdersData } from "../models/order.type";
import type { ApiResponse, ApiSingleResponse } from "../../../types/api.types";

export const orderApi = createApi({
    reducerPath: "orderApi",
    baseQuery: baseQueryWithErrorHandling,
    tagTypes: ["Orders"],
    endpoints: (builder) => ({
        // GET all orders for user
        fetchOrders: builder.query<PaginatedOrdersData, void>({
            query: () => "orders",
            transformResponse: (response: ApiResponse<OrderResponse>, meta: FetchBaseQueryMeta) => {
                const paginationHeader = meta?.response?.headers.get("Pagination");
                const pagination = paginationHeader ? JSON.parse(paginationHeader) : null;
                const data = response.data;
                return { response: data, pagination };
            },
            providesTags: ["Orders"],
        }),
        // GET a specific order by id
        fetchOrderDetailed: builder.query<OrderResponse, number>({
            query: (id) => ({
                url: `orders/${id}`,
            }),
        }),
        // CREATE order for user
        createOrder: builder.mutation<OrderResponse, OrderCreateDto>({
            query: (order) => ({
                url: "orders",
                method: "POST",
                body: order,
            }),
            transformResponse: (response: ApiSingleResponse<OrderResponse>) => response.data,
            onQueryStarted: async (_, { dispatch, queryFulfilled }) => {
                await queryFulfilled;
                dispatch(orderApi.util.invalidateTags(["Orders"]));
            },
        }),
    }),
});

export const { useFetchOrdersQuery, useFetchOrderDetailedQuery, useCreateOrderMutation } = orderApi;
