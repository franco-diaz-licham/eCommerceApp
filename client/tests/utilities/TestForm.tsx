import type { ReactNode } from "react";
import { useFormContext, type FieldValues, type SubmitHandler } from "react-hook-form";

type TestFormProps = {
    onSubmit?: SubmitHandler<FieldValues>;
    children: ReactNode;
    submitLabel?: string;
};

/** Dummy React form to render any field and test fo user actions. Useful when validating submit behvaiour.*/
export function TestForm({ onSubmit, children, submitLabel = "Submit" }: TestFormProps) {
    const { handleSubmit } = useFormContext();
    return (
        <form onSubmit={handleSubmit(onSubmit ?? (() => undefined))}>
            {children}
            <button type="submit">{submitLabel}</button>
        </form>
    );
}