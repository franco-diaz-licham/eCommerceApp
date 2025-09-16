import { createApi } from "@reduxjs/toolkit/query/react";
import { toast } from "react-toastify";
import type { LoginSchema } from "../types/loginSchema";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import type { Address, UserResponse } from "../types/user.type";
import { Routes } from "../../../app/routes/Routes";
import type { ApiSingleResponse } from "../../../types/api.types";

export const accountApi = createApi({
    reducerPath: "accountApi",
    baseQuery: baseQueryWithErrorHandling,
    tagTypes: ["UserInfo"],
    endpoints: (builder) => ({
        // LOGIN user with username/email and password.
        login: builder.mutation<UserResponse, LoginSchema>({
            query: (creds) => {
                return {
                    url: "account/login?useCookies=true",
                    method: "POST",
                    body: creds,
                };
            },
            transformResponse: (response: ApiSingleResponse<UserResponse>) => response.data,
            async onQueryStarted(_, { dispatch, queryFulfilled }) {
                try {
                    await queryFulfilled;
                    dispatch(accountApi.util.invalidateTags(["UserInfo"]));
                } catch (error) {
                    console.log(error);
                }
            },
        }),
        // REGISTER user with username/password
        register: builder.mutation<void, object>({
            query: (creds) => {
                return {
                    url: "account/register",
                    method: "POST",
                    body: creds,
                };
            },
            async onQueryStarted(_, { queryFulfilled }) {
                try {
                    await queryFulfilled;
                    toast.success("Registration successful - you can now sign in!");
                    Routes.navigate("/login");
                } catch (error) {
                    console.log(error);
                    throw error;
                }
            },
        }),
        // GET logged in user information.
        userInfo: builder.query<UserResponse, void>({
            query: () => "account/user",
            transformResponse: (response: ApiSingleResponse<UserResponse>) => response.data,
            providesTags: ["UserInfo"],
        }),
        // SIGNOUT currenly logged in user.
        logout: builder.mutation({
            query: () => ({
                url: "account/logout",
                method: "POST",
            }),
            async onQueryStarted(_, { dispatch, queryFulfilled }) {
                await queryFulfilled;
                dispatch(accountApi.util.invalidateTags(["UserInfo"]));
                Routes.navigate("/");
            },
        }),
        
        fetchAddress: builder.query<Address, void>({
            query: () => ({
                url: "account/address",
            }),
        }),
        updateUserAddress: builder.mutation<Address, Address>({
            query: (address) => ({
                url: "account/address",
                method: "POST",
                body: address,
            }),
            onQueryStarted: async (address, { dispatch, queryFulfilled }) => {
                const patchResult = dispatch(
                    accountApi.util.updateQueryData("fetchAddress", undefined, (draft) => {
                        Object.assign(draft, { ...address });
                    })
                );
                try {
                    await queryFulfilled;
                } catch (error) {
                    patchResult.undo();
                    console.log(error);
                }
            },
        }),
    }),
});

export const { useLoginMutation, useRegisterMutation, useLogoutMutation, useUserInfoQuery, useLazyUserInfoQuery, useFetchAddressQuery, useUpdateUserAddressMutation } = accountApi;
