import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Controller, useFormContext } from "react-hook-form";
import { renderWithForm } from "../utilities/RenderWithForm";
import { TestForm } from "../utilities/TestForm";
import CheckboxButtons from "../../src/components/ui/CheckboxButtons";

type Option = { value: number; label: string };

type SampleFormShape = {
    selections: Option[] | undefined;
};

/** Test-only adapter to hook into RHF */
function CheckboxField({ name, items, rules }: { name: keyof SampleFormShape & string; items: Option[]; rules?: Parameters<typeof Controller>[0]["rules"] }) {
    const { control } = useFormContext();
    return <Controller name={name} control={control} rules={rules} render={({ field }) => <CheckboxButtons items={items} checked={(field.value as Option[] | undefined) ?? []} onChange={(updated) => field.onChange(updated)} name={name} control={control} />} />;
}

const ITEMS: Option[] = [
    { value: 1, label: "Apples" },
    { value: 2, label: "Bananas" },
    { value: 3, label: "Cherries" },
];

describe("<CheckboxButtons />", () => {
    it("Should render all checkboxes and none selected when value is undefined", () => {
        // Arrange
        renderWithForm<SampleFormShape>(<CheckboxField name="selections" items={ITEMS} />, {
            defaultValues: { selections: undefined },
        });
        const apples = screen.getByRole("checkbox", { name: /apples/i });
        const bananas = screen.getByRole("checkbox", { name: /bananas/i });
        const cherries = screen.getByRole("checkbox", { name: /cherries/i });

        // Assert
        expect(apples).toBeInTheDocument();
        expect(bananas).toBeInTheDocument();
        expect(cherries).toBeInTheDocument();
        expect(apples).not.toBeChecked();
        expect(bananas).not.toBeChecked();
        expect(cherries).not.toBeChecked();
    });

    it("Should respect preset default values from defaultValues", () => {
        // Arrange
        renderWithForm<SampleFormShape>(<CheckboxField name="selections" items={ITEMS} />, {
            defaultValues: {
                selections: [{ value: 2, label: "Bananas" }],
            },
        });

        // Assert
        expect(screen.getByRole("checkbox", { name: /bananas/i })).toBeChecked();
        expect(screen.getByRole("checkbox", { name: /apples/i })).not.toBeChecked();
        expect(screen.getByRole("checkbox", { name: /cherries/i })).not.toBeChecked();
    });

    it("Should toggle a single checkbox and updates RHF state", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods } = renderWithForm<SampleFormShape>(<CheckboxField name="selections" items={ITEMS} />, {
            defaultValues: { selections: undefined },
        });

        const apples = screen.getByRole("checkbox", { name: /apples/i });

        // Act
        await user.click(apples);

        // Assert
        expect(apples).toBeChecked();
        expect(methods.getValues("selections")).toEqual([{ value: 1, label: "Apples" }]);
    });

    it("Should support multi-select and keeps all chosen items checked", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods } = renderWithForm<SampleFormShape>(<CheckboxField name="selections" items={ITEMS} />, {
            defaultValues: { selections: [] },
        });
        const apples = screen.getByRole("checkbox", { name: /apples/i });
        const cherries = screen.getByRole("checkbox", { name: /cherries/i });

        // Act
        await user.click(apples);
        await user.click(cherries);

        // Assert
        expect(apples).toBeChecked();
        expect(cherries).toBeChecked();
        expect(methods.getValues("selections")).toEqual(
            expect.arrayContaining([
                { value: 1, label: "Apples" },
                { value: 3, label: "Cherries" },
            ])
        );
    });

    it("Should deselect an already selected item", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods } = renderWithForm<SampleFormShape>(<CheckboxField name="selections" items={ITEMS} />, {
            defaultValues: { selections: [{ value: 1, label: "Apples" }] },
        });

        const apples = screen.getByRole("checkbox", { name: /apples/i });

        // Pre-Assert
        expect(apples).toBeChecked();

        // Act
        await user.click(apples);

        // Assert
        expect(apples).not.toBeChecked();
        expect(methods.getValues("selections")).toEqual([]);
    });

    it("Should blocks submit when required and empty; allows submit after selection", async () => {
        // Arrange
        const user = userEvent.setup();
        const onSubmit = vi.fn();
        renderWithForm<SampleFormShape>(
            <TestForm onSubmit={onSubmit} submitLabel="Submit">
                <CheckboxField
                    name="selections"
                    items={ITEMS}
                    rules={{
                        validate: (v: Option[] | undefined) => (v && v.length > 0 ? true : "Select at least one item"),
                    }}
                />
            </TestForm>,
            { defaultValues: { selections: [] } }
        );

        // Act: submit with no selections
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert
        expect(onSubmit).not.toHaveBeenCalled();

        // Act: select then submit
        await user.click(screen.getByRole("checkbox", { name: /bananas/i }));
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert
        expect(onSubmit).toHaveBeenCalledWith(expect.objectContaining({ selections: [{ value: 2, label: "Bananas" }] }), expect.anything());
    });

    it("syncs when external value changes (prop-driven via RHF)", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods } = renderWithForm<SampleFormShape>(<CheckboxField name="selections" items={ITEMS} />, {
            defaultValues: { selections: [{ value: 3, label: "Cherries" }] },
        });
        const cherries = screen.getByRole("checkbox", { name: /cherries/i });
        const apples = screen.getByRole("checkbox", { name: /apples/i });

        // Pre-Assert
        expect(cherries).toBeChecked();
        expect(apples).not.toBeChecked();

        // Act: external change through RHF (mimics parent prop update) and user then toggles one more
        methods.setValue("selections", [{ value: 1, label: "Apples" }]);
        await user.click(cherries);

        // Assert: multi-select now includes both
        expect(apples).toBeChecked();
        expect(cherries).toBeChecked();
        expect(methods.getValues("selections")).toEqual(
            expect.arrayContaining([
                { value: 1, label: "Apples" },
                { value: 3, label: "Cherries" },
            ])
        );
    });

    it("Should keep internal controlled state consistent when value is undefined (no boxes checked)", () => {
        // Arrange
        renderWithForm<SampleFormShape>(<CheckboxField name="selections" items={ITEMS} />, {
            defaultValues: { selections: undefined },
        });

        // Assert
        expect(screen.getByRole("checkbox", { name: /apples/i })).not.toBeChecked();
        expect(screen.getByRole("checkbox", { name: /bananas/i })).not.toBeChecked();
        expect(screen.getByRole("checkbox", { name: /cherries/i })).not.toBeChecked();
    });
});
