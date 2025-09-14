import { createApi } from "@reduxjs/toolkit/query/react";
import type { BasketResponse } from "../../basket/types/basket.type";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import { basketApi } from "../../basket/services/basket.api";

export const checkoutApi = createApi({
    reducerPath: "checkoutApi",
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        createPaymentIntent: builder.mutation<BasketResponse, void>({
            query: () => {
                return {
                    url: "payments",
                    method: "POST",
                };
            },
            onQueryStarted: async (_, { dispatch, queryFulfilled }) => {
                try {
                    const { data } = await queryFulfilled;
                    const basketId = localStorage.getItem("basketId");
                    if (!basketId) return;
                    dispatch(
                        basketApi.util.updateQueryData("fetchBasket", Number(basketId), (draft) => {
                            draft.clientSecret = data.clientSecret;
                        })
                    );
                } catch (error) {
                    console.log("Payment intent creation failed: ", error);
                }
            },
        }),
    }),
});

export const { useCreatePaymentIntentMutation } = checkoutApi;
