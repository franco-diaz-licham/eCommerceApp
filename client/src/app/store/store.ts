import { configureStore } from "@reduxjs/toolkit";
import { useDispatch, useSelector } from "react-redux";
import { productApi } from "../../features/product/services/product.api";
import { uiSlice } from "../../components/layout/uiSlice";
import { errorApi } from "../../features/error/services/error.api";

export const store = configureStore({
    reducer: {
        [productApi.reducerPath]: productApi.reducer,
        [errorApi.reducerPath]: errorApi.reducer,
        ui: uiSlice.reducer,
    },
    middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(productApi.middleware, errorApi.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
export const useAppDispatch = useDispatch.withTypes<AppDispatch>();
export const useAppSelector = useSelector.withTypes<RootState>();
