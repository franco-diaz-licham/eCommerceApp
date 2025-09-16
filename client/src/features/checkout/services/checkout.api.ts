import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import { basketApi } from "../../basket/services/basket.api";
import type { PaymentIntentResponse } from "../types/checkout.type";
import type { ApiSingleResponse } from "../../../types/api.types";

export const checkoutApi = createApi({
    reducerPath: "checkoutApi",
    baseQuery: baseQueryWithErrorHandling,
    endpoints: (builder) => ({
        // CREATE paymentintent with stripe.
        createPaymentIntent: builder.mutation<PaymentIntentResponse, number>({
            query: (basketId) => {
                return {
                    url: `payment?basketId=${basketId}`,
                    method: "POST",
                };
            },
            transformResponse: (response: ApiSingleResponse<PaymentIntentResponse>) => response.data,
            onQueryStarted: async (_, { dispatch, queryFulfilled }) => {
                try {
                    const { data: response } = await queryFulfilled;
                    const basketId = localStorage.getItem("basketId");
                    if (!basketId) return;
                    dispatch(
                        basketApi.util.updateQueryData("fetchBasket", Number(basketId), (draft) => {
                            draft.clientSecret = response.clientSecret;
                            draft.paymentIntentId = response.paymentIntent;
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
