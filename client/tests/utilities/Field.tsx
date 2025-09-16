import type { InputHTMLAttributes } from "react";
import { useFormContext, type FieldValues, type Path, type RegisterOptions } from "react-hook-form";
import TextInput from "../../src/components/ui/TextInput";

type FieldProps<T extends FieldValues> = {
    name: Path<T>;
    label?: string;
    rules?: RegisterOptions<T, Path<T>>;
    type?: InputHTMLAttributes<HTMLInputElement>["type"];
    multiline?: boolean;
    rows?: number;
};

/** Enables easy field setup for testing of TextInput. */
export function Field<T extends FieldValues>(props: FieldProps<T>) {
    const { control } = useFormContext<T>();
    return <TextInput<T> name={props.name} control={control} rules={props.rules} label={props.label} type={props.type} multiline={props.multiline} rows={props.rows} />;
}
