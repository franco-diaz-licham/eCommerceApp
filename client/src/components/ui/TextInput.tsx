import { TextField, type TextFieldProps } from "@mui/material";
import type { ChangeEvent } from "react";
import { type FieldValues, useController, type UseControllerProps } from "react-hook-form";

type Props<T extends FieldValues> = UseControllerProps<T> & TextFieldProps;

export default function TextInput<T extends FieldValues>(props: Props<T>) {
    const { fieldState, field } = useController(props);

    const handleChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const raw = e.target.value as string;
        if (props.type === "number") {
            field.onChange(raw === "" ? undefined : Number(raw));
            return;
        }
        field.onChange(raw);
    };

    return <TextField {...props} {...field} multiline={props.multiline} rows={props.rows} type={props.type} fullWidth value={field.value ?? ""} variant="outlined" error={!!fieldState.error} helperText={fieldState.error?.message} onChange={handleChange} />;
}
