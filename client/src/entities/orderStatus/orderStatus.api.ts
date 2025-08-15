import { createApi, type FetchBaseQueryMeta } from "@reduxjs/toolkit/query/react";
import type { Pagination } from "../../types/pagination.type";
import type { BaseQueryParams } from "../../types/baseQueryParams.type";
import { filterEmptyValues } from "../../lib/utils";
import type { ApiResponse } from "../../types/api.types";
import type { OrderStatusResponse } from "./orderStatus.type";
import { baseQueryWithErrorHandling } from "../../app/providers/base.api";

/** Base url resource endpoint. */
const baseUrl: string = "OrderStatuses";
interface mainOrderStatussData {
    response: OrderStatusResponse[];
    pagination: Pagination;
}

/** Get OrderStatuss handler. */
const getOrderStatuss = (OrderStatusParams: BaseQueryParams) => {
    return {
        url: baseUrl,
        params: filterEmptyValues(OrderStatusParams),
    };
};

/** Response list handler. */
const transformPaginatedResponse = (response: ApiResponse<OrderStatusResponse>, meta: FetchBaseQueryMeta) => {
    const paginationHeader = meta?.response?.headers.get("Pagination");
    const pagination = paginationHeader ? JSON.parse(paginationHeader) : null;
    const data = response.data;
    return { response: data, pagination };
};

/** OrderStatus redux configuration */
export const OrderStatusApi = createApi({
    reducerPath: "OrderStatusApi",
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        fetchOrderStatuss: builder.query<mainOrderStatussData, BaseQueryParams>({
            query: getOrderStatuss,
            transformResponse: transformPaginatedResponse,
        }),
    }),
});

export const { useFetchOrderStatussQuery } = OrderStatusApi;
