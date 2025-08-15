import { createApi, type FetchBaseQueryMeta } from "@reduxjs/toolkit/query/react";
import type { BrandResponse } from "./brand.types";
import type { Pagination } from "../../types/pagination.type";
import type { BaseQueryParams } from "../../types/baseQueryParams.type";
import { filterEmptyValues } from "../../lib/utils";
import type { ApiResponse } from "../../types/api.types";
import { baseQueryWithErrorHandling } from "../../app/providers/base.api";

/** Base url resource endpoint. */
const baseUrl: string = "Brands";
interface mainBrandsData {
    response: BrandResponse[];
    pagination: Pagination;
}

/** Get Brands handler. */
const getBrands = (BrandParams: BaseQueryParams) => {
    return {
        url: baseUrl,
        params: filterEmptyValues(BrandParams),
    };
};

/** Response list handler. */
const transformPaginatedResponse = (response: ApiResponse<BrandResponse>, meta: FetchBaseQueryMeta) => {
    const paginationHeader = meta?.response?.headers.get("Pagination");
    const pagination = paginationHeader ? JSON.parse(paginationHeader) : null;
    const data = response.data;
    return { response: data, pagination };
};

/** Brand redux configuration */
export const BrandApi = createApi({
    reducerPath: "BrandApi",
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        fetchBrands: builder.query<mainBrandsData, BaseQueryParams>({
            query: getBrands,
            transformResponse: transformPaginatedResponse,
        }),
    }),
});

export const { useFetchBrandsQuery } = BrandApi;
