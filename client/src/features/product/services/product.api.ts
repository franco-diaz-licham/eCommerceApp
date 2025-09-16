import { createApi, type FetchBaseQueryMeta } from "@reduxjs/toolkit/query/react";
import { createFormData } from "../../../lib/utils";
import type { PaginatedProductsData, ProductCreate, ProductFilters, ProductQueryParams, ProductResponse, ProductUpdate } from "../types/product.types";
import type { ApiResponse, ApiSingleResponse } from "../../../types/api.types";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";

/** Base url resource endpoint. */
const baseUrl: string = "products";

/** Response list handler. */
const transformPaginatedResponse = (response: ApiResponse<ProductResponse>, meta: FetchBaseQueryMeta) => {
    const paginationHeader = meta?.response?.headers.get("Pagination");
    const pagination = paginationHeader ? JSON.parse(paginationHeader) : null;
    const data = response.data;
    return { response: data, pagination };
};

/** Product reducer configuration. */
export const productApi = createApi({
    reducerPath: "productApi",
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        /** GET all products. */
        fetchProducts: builder.query<PaginatedProductsData, ProductQueryParams>({
            query: (productParams: ProductQueryParams) => {
                return {
                    url: baseUrl,
                    params: productParams,
                };
            },
            transformResponse: transformPaginatedResponse,
        }),
        /** GET product filters handler. */
        fetchProductDetails: builder.query<ProductResponse, number>({
            query: (productId: number) => `${baseUrl}/${productId}`,
            transformResponse: (response: ApiSingleResponse<ProductResponse>) => response.data,
        }),
        /** GET product filters handler. */
        fetchFilters: builder.query<ProductFilters, void>({
            query: () => `${baseUrl}/filters`,
        }),
        /** CREATE a product. */
        createProduct: builder.mutation<ProductResponse, ProductCreate>({
            query: (data: ProductCreate) => {
                const formData = createFormData(data);
                return {
                    url: baseUrl,
                    method: "POST",
                    body: formData,
                };
            },
        }),
        /** UPDATE a product. */
        updateProduct: builder.mutation<ProductResponse, ProductUpdate>({
            query: (data: ProductUpdate) => {
                const formData = createFormData(data);
                return {
                    url: baseUrl,
                    method: "PUT",
                    body: formData,
                };
            },
        }),
        /** DELETE a product. */
        deleteProduct: builder.mutation<void, number>({
            query: (id: number) => {
                return {
                    url: `${baseUrl}/${id}`,
                    method: "DELETE",
                };
            },
        }),
    }),
});

export const { useFetchProductDetailsQuery, useFetchProductsQuery, useFetchFiltersQuery, useCreateProductMutation, useUpdateProductMutation, useDeleteProductMutation } = productApi;
