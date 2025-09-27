import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import type { OrderCreateDto, OrderResponse, PaginatedOrdersData } from "../models/order.type";
import type { ApiSingleResponse } from "../../../types/api.types";
import type { BaseQueryParams } from "../../../types/baseQueryParams.type";
import { transformPaginatedResponse, transformSingleResponse } from "../../../lib/utils";

/** Base url resource endpoint. */
const baseUrl: string = "orders";

export const orderApi = createApi({
    reducerPath: "orderApi",
    baseQuery: baseQueryWithErrorHandling,
    tagTypes: ["Orders"],
    endpoints: (builder) => ({
        // GET all orders for user
        fetchOrders: builder.query<PaginatedOrdersData, BaseQueryParams>({
            query: (ordersParams: BaseQueryParams) => {
                return {
                    url: baseUrl,
                    params: ordersParams,
                };
            },
            transformResponse: transformPaginatedResponse,
            providesTags: ["Orders"],
        }),
        // GET a specific order by id
        fetchOrderDetailed: builder.query<OrderResponse, number>({
            query: (id) => ({
                url: `${baseUrl}/${id}`,
            }),
            transformResponse: transformSingleResponse,
        }),
        // CREATE order for user
        createOrder: builder.mutation<OrderResponse, OrderCreateDto>({
            query: (order) => ({
                url: baseUrl,
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
