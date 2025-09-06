import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import type { ProductCreate } from "../../product/types/product.types";

export const adminApi = createApi({
    reducerPath: 'adminApi',
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        createProduct: builder.mutation<ProductCreate, FormData>({
            query: (data: FormData) => {
                return {
                    url: 'products',
                    method: 'POST',
                    body: data
                }
            }
        }),
        updateProduct: builder.mutation<void, {id: number, data: FormData}>({
            query: ({id, data}) => {
                data.append('id', id.toString())

                return {
                    url: 'products',
                    method: 'PUT',
                    body: data
                }
            }
        }),
        deleteProduct: builder.mutation<void, number>({
            query: (id: number) => {
                return {
                    url: `products/${id}`,
                    method: 'DELETE'
                }
            }
        })
    })
});

export const {useCreateProductMutation, useUpdateProductMutation, useDeleteProductMutation} = adminApi;