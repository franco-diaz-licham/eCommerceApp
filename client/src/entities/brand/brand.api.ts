import { createApi } from "@reduxjs/toolkit/query/react";
import type { BrandResponse } from "./brand.types";
import type { Pagination } from "../../types/pagination.type";
import type { BaseQueryParams } from "../../types/baseQueryParams.type";
import { baseQueryWithErrorHandling } from "../../app/providers/base.api";
import { transformPaginatedResponse } from "../../lib/utils";

/** Base url resource endpoint. */
const baseUrl: string = "Brands";
interface mainBrandsData {
    response: BrandResponse[];
    pagination: Pagination;
}

/** Brand redux configuration */
export const BrandApi = createApi({
    reducerPath: "BrandApi",
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        fetchBrands: builder.query<mainBrandsData, BaseQueryParams>({
            query: (BrandParams: BaseQueryParams) => {
                return {
                    url: baseUrl,
                    params: BrandParams,
                };
            },
            transformResponse: transformPaginatedResponse,
        }),
    }),
});

export const { useFetchBrandsQuery } = BrandApi;
