import { createSlice } from "@reduxjs/toolkit";
import type { ProductQueryParams } from "../models/product.types";

/** Pageginated intial state. */
const initialState: ProductQueryParams = {
    pageNumber: 1,
    pageSize: 12,
    productTypeIds: [],
    brandIds: [],
    searchTerm: "",
    orderBy: "name",
};

export const productSlice = createSlice({
    name: "productSlice",
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
        setTypes(state, action) {
            state.productTypeIds = action.payload;
            state.pageNumber = 1;
        },
        setBrands(state, action) {
            state.brandIds = action.payload;
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

export const { setBrands, setOrderBy, setPageNumber, setPageSize, setSearchTerm, setTypes, resetParams } = productSlice.actions;
