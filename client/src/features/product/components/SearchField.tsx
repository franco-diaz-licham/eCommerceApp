import { debounce, TextField } from "@mui/material";
import { useEffect, useState, type ChangeEvent } from "react";
import { useAppDispatch, useAppSelector } from "../../../app/store/store";
import { setSearchTerm } from "../services/productSlice";

export default function SearchField() {
    const { searchTerm } = useAppSelector((state) => state.products);
    const dispatch = useAppDispatch();
    const [term, setTerm] = useState(searchTerm);

    useEffect(() => {
        setTerm(searchTerm);
    }, [searchTerm]);

    /** Wait every 500ms for next search. */
    const debouncedSearch = debounce((event) => {
        dispatch(setSearchTerm(event.target.value));
    }, 500);

    /** Handle on search box. */
    const handleOnChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        setTerm(e.target.value);
        debouncedSearch(e);
    };

    return <TextField label="Search products" variant="outlined" fullWidth type="search" value={term} onChange={handleOnChange} />;
}
