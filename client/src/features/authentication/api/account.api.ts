import { createApi } from "@reduxjs/toolkit/query/react";
import { toast } from "react-toastify";
import type { LoginSchema } from "../models/loginSchema";
import { baseQueryWithErrorHandling } from "../../../app/providers/base.api";
import type { AddressCreateDto, AddressResponse, AddressUpdateDto, UserResponse } from "../models/user.type";
import { Routes } from "../../../app/routes/Routes";
import type { ApiSingleResponse } from "../../../types/api.types";
import { nothing } from "immer";

export const accountApi = createApi({
    reducerPath: "accountApi",
    baseQuery: baseQueryWithErrorHandling,
    tagTypes: ["UserInfo"],
    endpoints: (builder) => ({
        // LOGIN user with username/email and password.
        login: builder.mutation<UserResponse, LoginSchema>({
            query: (credentials) => {
                return {
                    url: "account/login?useCookies=true",
                    method: "POST",
                    body: credentials,
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
        signOut: builder.mutation({
            query: () => ({
                url: "account/signout",
                method: "POST",
            }),
            async onQueryStarted(_, { dispatch, queryFulfilled }) {
                dispatch(accountApi.util.updateQueryData("userInfo", undefined, () => nothing as unknown as UserResponse));

                try {
                    await queryFulfilled;
                } finally {
                    Routes.navigate("/");
                }
            },
        }),
        fetchAddress: builder.query<AddressResponse, void>({
            query: () => ({
                url: "account/address",
            }),
        }),
        updateUserAddress: builder.mutation<AddressResponse, AddressUpdateDto>({
            query: (address) => ({
                url: "account/address",
                method: "PUT",
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
        createUserAddress: builder.mutation<AddressResponse, AddressCreateDto>({
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

export const { useLoginMutation, useRegisterMutation, useSignOutMutation, useUserInfoQuery, useLazyUserInfoQuery, useFetchAddressQuery, useUpdateUserAddressMutation, useCreateUserAddressMutation } = accountApi;
