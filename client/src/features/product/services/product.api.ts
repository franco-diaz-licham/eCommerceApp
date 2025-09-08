import { createApi, type FetchBaseQueryMeta } from "@reduxjs/toolkit/query/react";
import type { Pagination } from "../../../types/pagination.type";
import { createFormData, filterEmptyValues } from "../../../lib/utils";
import type { ProductCreate, ProductFilters, ProductQueryParams, ProductResponse, ProductUpdate } from "../types/product.types";
import type { ApiResponse, ApiSingleResponse } from "../../../types/api.types";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";

/** Base url resource endpoint. */
const baseUrl: string = "products";
interface mainProductsData {
    response: ProductResponse[];
    pagination: Pagination;
}

/** Response list handler. */
const transformPaginatedResponse = (response: ApiResponse<ProductResponse>, meta: FetchBaseQueryMeta) => {
    const paginationHeader = meta?.response?.headers.get("Pagination");
    const pagination = paginationHeader ? JSON.parse(paginationHeader) : null;
    const data = response.data;
    return { response: data, pagination };
};

/** Response single handler. */
const transformPaginatedSingleResponse = (response: ApiSingleResponse<ProductResponse>) => response.data;

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

/** Creates a product. */
const createProduct = (data: ProductCreate) => {
    const formData = createFormData(data);
    return {
        url: "products",
        method: "POST",
        body: formData,
    };
};

/** Updates a product. */
const updateProduct = (data: ProductUpdate) => {
    const formData = createFormData(data);
    return {
        url: "products",
        method: "PUT",
        body: formData,
    };
};

/** Deletes a product. */
const deleteProduct = (id: number) => {
    return {
        url: `products/${id}`,
        method: "DELETE",
    };
};

/** Product reducer configuration. */
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
        fetchFilters: builder.query<ProductFilters, void>({
            query: getProductFilters,
        }),
        createProduct: builder.mutation<ProductResponse, ProductCreate>({
            query: (data) => createProduct(data),
        }),
        updateProduct: builder.mutation<ProductResponse, ProductUpdate>({
            query: (data) => updateProduct(data),
        }),
        deleteProduct: builder.mutation<void, number>({
            query: (id: number) => deleteProduct(id),
        }),
    }),
});

export const { useFetchProductDetailsQuery, useFetchProductsQuery, useFetchFiltersQuery, useCreateProductMutation, useUpdateProductMutation, useDeleteProductMutation } = productApi;
