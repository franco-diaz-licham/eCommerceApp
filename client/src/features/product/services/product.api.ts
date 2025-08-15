import { createApi, type FetchBaseQueryMeta } from "@reduxjs/toolkit/query/react";
import type { Pagination } from "../../../types/pagination.type";
import { filterEmptyValues } from "../../../lib/utils";
import type { ProductQueryParams, ProductResponse } from "../types/product.types";
import type { ApiResponse, ApiSingleResponse } from "../../../types/api.types";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";

/** Base url resource endpoint. */
const baseUrl: string = "products";
interface mainProductsData {
    response: ProductResponse[];
    pagination: Pagination;
}

/** Get products handler. */
const getProducts = (productParams: ProductQueryParams) => {
    return {
        url: baseUrl,
        params: filterEmptyValues(productParams),
    };
};

/** Get product filters handler. */
const getProduct = (productId: number) => `${baseUrl}/${productId}`;

/** Get product filters handler. */
const getProductFilters = () => `${baseUrl}/filters`;

/** Response list handler. */
const transformPaginatedResponse = (response: ApiResponse<ProductResponse>, meta: FetchBaseQueryMeta) => {
    const paginationHeader = meta?.response?.headers.get("Pagination");
    const pagination = paginationHeader ? JSON.parse(paginationHeader) : null;
    const data = response.data;
    return { response: data, pagination };
};

/** Response single handler. */
const transformPaginatedSingleResponse = (response: ApiSingleResponse<ProductResponse>) => response.data;

/** Product redux configuration */
export const productApi = createApi({
    reducerPath: "productApi",
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        fetchProducts: builder.query<mainProductsData, ProductQueryParams>({
            query: getProducts,
            transformResponse: transformPaginatedResponse,
        }),
        fetchProductDetails: builder.query<ProductResponse, number>({
            query: getProduct,
            transformResponse: transformPaginatedSingleResponse,
        }),
        fetchFilters: builder.query<{ brands: string[]; types: string[] }, void>({
            query: getProductFilters,
        }),
    }),
});

export const { useFetchProductDetailsQuery, useFetchProductsQuery, useFetchFiltersQuery } = productApi;
