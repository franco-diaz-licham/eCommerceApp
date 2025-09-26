import { createApi, type FetchBaseQueryMeta } from "@reduxjs/toolkit/query/react";
import type { Pagination } from "../../types/pagination.type";
import type { BaseQueryParams } from "../../types/baseQueryParams.type";
import type { ApiResponse } from "../../types/api.types";
import type { ProductTypeResponse } from "./productTypeResponse.type";
import { baseQueryWithErrorHandling } from "../../app/providers/base.api";

/** Base url resource endpoint. */
const baseUrl: string = "ProductTypes";
interface mainProductTypesData {
    response: ProductTypeResponse[];
    pagination: Pagination;
}

/** Get ProductTypes handler. */
const getProductTypes = (ProductTypeParams: BaseQueryParams) => {
    return {
        url: baseUrl,
        params: ProductTypeParams,
    };
};

/** Response list handler. */
const transformPaginatedResponse = (response: ApiResponse<ProductTypeResponse>, meta: FetchBaseQueryMeta) => {
    const paginationHeader = meta?.response?.headers.get("Pagination");
    const pagination = paginationHeader ? JSON.parse(paginationHeader) : null;
    const data = response.data;
    return { response: data, pagination };
};

/** ProductType redux configuration */
export const ProductTypeApi = createApi({
    reducerPath: "ProductTypeApi",
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        fetchProductTypes: builder.query<mainProductTypesData, BaseQueryParams>({
            query: getProductTypes,
            transformResponse: transformPaginatedResponse,
        }),
    }),
});

export const { useFetchProductTypesQuery } = ProductTypeApi;
