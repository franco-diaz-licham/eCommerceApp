import { debounce, TextField } from "@mui/material";
import { useMemo, useState, type ChangeEvent } from "react";

export type SearchFieldProps = {
    value: string;
    onSearchChange: (value: string) => void;
};

export default function SearchField({ value, onSearchChange }: SearchFieldProps) {
    const [term, setTerm] = useState(value);

    // stable debounced handler
    const debouncedSearch = useMemo(() => debounce((nextValue: string) => onSearchChange(nextValue), 500), [onSearchChange]);

    const handleOnChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const nextValue = e.target.value;
        setTerm(nextValue);
        debouncedSearch(nextValue);
    };

    return <TextField label="Search products" variant="outlined" fullWidth type="search" size="small" value={term} onChange={handleOnChange} />;
}
