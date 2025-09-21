import { configureStore } from "@reduxjs/toolkit";
import { useDispatch, useSelector } from "react-redux";
import { productApi } from "../../features/product/api/product.api";
import { uiSlice } from "../../components/layout/uiSlice";
import { errorApi } from "../../features/error/api/error.api";
import { productSlice } from "../../features/product/api/productSlice";
import { accountApi } from "../../features/authentication/api/account.api";
import { orderApi } from "../../features/order/api/orderApi";
import { basketApi } from "../../features/basket/api/basket.api";
import { basketSessionSlice } from "../../features/basket/api/basketSlice";
import { checkoutApi } from "../../features/checkout/api/checkout.api";

export const store = configureStore({
    reducer: {
        [productApi.reducerPath]: productApi.reducer,
        [errorApi.reducerPath]: errorApi.reducer,
        [basketApi.reducerPath]: basketApi.reducer,
        [checkoutApi.reducerPath]: checkoutApi.reducer,
        [accountApi.reducerPath]: accountApi.reducer,
        [orderApi.reducerPath]: orderApi.reducer,
        ui: uiSlice.reducer,
        products: productSlice.reducer,
        basketSession: basketSessionSlice.reducer,
    },
    middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(
        productApi.middleware, 
        errorApi.middleware, 
        orderApi.middleware, 
        accountApi.middleware, 
        basketApi.middleware, 
        checkoutApi.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = useDispatch.withTypes<AppDispatch>();
export const useAppSelector = useSelector.withTypes<RootState>();
