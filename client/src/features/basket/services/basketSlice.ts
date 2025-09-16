// basketSessionSlice.ts
import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

/** local storage basketId key */
const BASKET_ID_KEY = "basketId";

/** Get current id. */
const loadId = (): number | null => {
    const raw = localStorage.getItem(BASKET_ID_KEY);
    const n = raw ? Number(raw) : NaN;
    return Number.isFinite(n) && n > 0 ? n : null;
};

type State = { id: number | null };
const initialState: State = { id: loadId() };

export const basketSessionSlice = createSlice({
    name: "basketSession",
    initialState,
    reducers: {
        /** sets basket id into local storage. */
        setBasketId(state, action: PayloadAction<number | null>) {
            state.id = action.payload;
            if (action.payload == null) localStorage.removeItem(BASKET_ID_KEY);
            else localStorage.setItem(BASKET_ID_KEY, String(action.payload));
        }
    },
});

export const { setBasketId } = basketSessionSlice.actions;
export default basketSessionSlice.reducer;
