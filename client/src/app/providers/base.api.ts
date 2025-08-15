import { toast } from "react-toastify";
import { fetchBaseQuery, type BaseQueryApi, type FetchArgs } from "@reduxjs/toolkit/query/react";
import { Routes } from "../routes/Routes";
import { startLoading, stopLoading } from "../../components/layout/uiSlice";
import { StatusCode, type ErrorApiResponse } from "../../types/api.types";

/** Create custom base query */
const customBaseQuery = fetchBaseQuery({
    baseUrl: import.meta.env.VITE_API_URL,
});

/** Development delay for loading time. */
const sleep = () => new Promise((resolve) => setTimeout(resolve, 1000));

const setLoaders = async (args: string | FetchArgs, api: BaseQueryApi, extraOptions: object) => {
    api.dispatch(startLoading());
    if (import.meta.env.DEV) await sleep();
    const result = await customBaseQuery(args, api, extraOptions);
    api.dispatch(stopLoading());
    return result;
};

/** Central api query handler. */
export const baseQueryWithErrorHandling = async (args: string | FetchArgs, api: BaseQueryApi, extraOptions: object) => {
    const result = await setLoaders(args, api, extraOptions);
    // Check error result
    if (result.error) {
        const originalStatus = result.error.status === "PARSING_ERROR" && result.error.originalStatus ? result.error.originalStatus : result.error.status;
        const responseData = result.error.data as ErrorApiResponse;
        console.log(responseData);
        
        switch (originalStatus) {
            case StatusCode.BadRequest:
                if ("validationErrors" in responseData) toast.error(responseData.validationErrors!.flat().join(" "));
                else toast.error(responseData.message);
                break;
            case StatusCode.Unauthorized:
                if (typeof responseData === "object" && "message" in responseData) toast.error(responseData.message);
                break;
            case StatusCode.Forbidden:
                if (typeof responseData === "object") toast.error("403 Forbidden");
                break;
            case StatusCode.NotFound:
                if (typeof responseData === "object" && "message" in responseData) toast.error(responseData.message);
                break;
            case StatusCode.ServerError:
                if (typeof responseData === "object") Routes.navigate("/server-error", { state: { error: responseData } });
                break;
            default:
                break;
        }
    }

    return result;
};
