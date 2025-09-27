import { createSlice } from "@reduxjs/toolkit";
import type { BaseQueryParams } from "../../../types/baseQueryParams.type";

/** Pageginated intial state. */
const initialState: BaseQueryParams = {
    pageNumber: 1,
    pageSize: 12,
    searchTerm: "",
    orderBy: "name",
};

export const orderSlice = createSlice({
    name: "orderSlice",
    initialState,
    reducers: {
        setPageNumber(state, action) {
            state.pageNumber = action.payload;
        },
        setPageSize(state, action) {
            state.pageSize = action.payload;
        },
        setOrderBy(state, action) {
            state.orderBy = action.payload;
            state.pageNumber = 1;
        },
        setSearchTerm(state, action) {
            state.searchTerm = action.payload;
            state.pageNumber = 1;
        },
        resetParams() {
            return initialState;
        },
    },
});

export const { setOrderBy, setPageNumber, setPageSize, setSearchTerm, resetParams } = orderSlice.actions;
