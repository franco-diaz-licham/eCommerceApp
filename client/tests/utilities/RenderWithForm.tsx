import { render } from "@testing-library/react";
import type { FC, ReactElement, ReactNode } from "react";
import { FormProvider, useForm, type DefaultValues, type FieldValues, type UseFormReturn } from "react-hook-form";

/** Renders a React element inside a FormProvider and returns testing-library utilities plus all the useForm() methods. */
export function renderWithForm<T extends FieldValues>(ui: ReactElement, opts?: { defaultValues?: DefaultValues<T> }) {
    let captured!: UseFormReturn<T>;

    const FormWrapper: FC<{ children: ReactNode }> = ({ children }) => {
        const methods = useForm<T>({
            defaultValues: opts?.defaultValues,
        });
        captured = methods;
        return <FormProvider {...methods}>{children}</FormProvider>;
    };

    const utils = render(ui, { wrapper: FormWrapper });
    return { ...utils, methods: captured };
}
