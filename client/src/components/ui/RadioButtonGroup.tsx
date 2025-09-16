import { FormControl, FormControlLabel, Radio, RadioGroup, type RadioProps } from "@mui/material";
import { type ChangeEvent } from "react";
import type { FieldValues, UseControllerProps } from "react-hook-form";

/** Functional props. Add base RHF and MUI props. */
type RadioButtonGroupProps<T extends FieldValues> = {
    options: { value: string; label: string }[];
    onChange: (event: ChangeEvent<HTMLInputElement>) => void;
    selectedValue: string;
} & UseControllerProps<T> &
    Partial<RadioProps>;

export default function RadioButtonGroup<T extends FieldValues>(props: RadioButtonGroupProps<T>) {
    const radioId = `${props.name}-radio`;

    return (
        <FormControl>
            <RadioGroup onChange={props.onChange} value={props.selectedValue} sx={{ my: 0 }} id={radioId}>
                {props.options.map(({ value, label }) => (
                    <FormControlLabel key={label} control={<Radio color="secondary" sx={{ py: 0.7 }} />} label={label} value={value} />
                ))}
            </RadioGroup>
        </FormControl>
    );
}
