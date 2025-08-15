import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";

const baseUrl: string = "buggy";

export const errorApi = createApi({
    reducerPath: "errorApi",
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        get400Error: builder.query<void, void>({
            query: () => ({ url: `${baseUrl}/bad-request` }),
        }),
        get401Error: builder.query<void, void>({
            query: () => ({ url: `${baseUrl}/unauthorized` }),
        }),
        get404Error: builder.query<void, void>({
            query: () => ({ url: `${baseUrl}/not-found` }),
        }),
        get500Error: builder.query<void, void>({
            query: () => ({ url: `${baseUrl}/server-error` }),
        }),
        postValidationError: builder.mutation<void, void>({
            query: () => ({
                url: `${baseUrl}/validation-error`,
                method: "POST",
                body: { required1: null, required2: null },
            }),
        }),
    }),
});
export const { useLazyGet400ErrorQuery, useLazyGet401ErrorQuery, useLazyGet500ErrorQuery, useLazyGet404ErrorQuery, usePostValidationErrorMutation } = errorApi;
