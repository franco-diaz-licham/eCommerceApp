import type { ReactNode } from "react";
import { useFormContext, type FieldValues, type SubmitHandler } from "react-hook-form";

type TestFormProps = {
    onSubmit?: SubmitHandler<FieldValues>;
    children: ReactNode;
    submitLabel?: string;
};

/** Dummy React component to render any field inside a Form component and test for user actions. Useful when validating submit behaviour.*/
export function TestForm({ onSubmit, children, submitLabel = "Submit" }: TestFormProps) {
    const { handleSubmit } = useFormContext();
    return (
        <form onSubmit={handleSubmit(onSubmit ?? (() => undefined))}>
            {children}
            <button type="submit">{submitLabel}</button>
        </form>
    );
}