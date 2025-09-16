/* eslint-disable @typescript-eslint/no-explicit-any */
import { render } from "@testing-library/react";
import type { FC, ReactElement, ReactNode } from "react";
import { FormProvider, useForm, type DefaultValues, type FieldValues, type UseFormReturn } from "react-hook-form";

/** Renders a React element inside a FormProvider and returns testing-library utilities plus all the useForm() methods.
 * Useful when validation form state. */
export function renderWithForm<T extends FieldValues>(ui: ReactElement, opts?: { defaultValues?: DefaultValues<T>; resolver?: any }) {
    let methods!: UseFormReturn<T>;

    const FormWrapper: FC<{ children: ReactNode }> = ({ children }) => {
        methods = useForm<T>({
            defaultValues: opts?.defaultValues,
            resolver: opts?.resolver,
        });
        return <FormProvider {...methods}>{children}</FormProvider>;
    };

    const utils = render(ui, { wrapper: FormWrapper });
    return { ...utils, methods: methods };
}
