/* eslint-disable @typescript-eslint/no-explicit-any */
import { FormControl, FormHelperText, InputLabel, MenuItem, Select, type SelectProps } from "@mui/material";
import { type FieldValues, useController, type UseControllerProps } from "react-hook-form";

/** Option model for selector */
type Option = { value: number; label: string };

/** Functional props. */
type SelectInputProps<T extends FieldValues> = {
    items: Option[];
} & UseControllerProps<T> &
    Partial<SelectProps>;

export default function SelectInput<TFieldValues extends FieldValues>(props: SelectInputProps<TFieldValues>) {
    const { items, ...controllerProps } = props;
    const { field, fieldState } = useController(controllerProps);

    /** MUI SelectChangeEvent.value is a string; convert to number */
    const handleChange = (e: any) => {
        const raw = e.target.value as string;
        field.onChange(raw === "" ? undefined : Number(raw));
    };

    // Convert Select value is a string for MUI and convert from number
    const value = field.value == null ? "" : String(field.value);

    return (
        <FormControl fullWidth error={!!fieldState.error}>
            <InputLabel>{props.label}</InputLabel>
            <Select label={props.label} value={value} onChange={handleChange}>
                <MenuItem value="">
                    <em>Select {props.label}</em>
                </MenuItem>
                {items.map((opt) => (
                    <MenuItem value={String(opt.value)} key={opt.value}>
                        {opt.label}
                    </MenuItem>
                ))}
            </Select>
            <FormHelperText>{fieldState.error?.message}</FormHelperText>
        </FormControl>
    );
}
