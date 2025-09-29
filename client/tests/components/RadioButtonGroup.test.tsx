import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Controller, useFormContext } from "react-hook-form";
import { renderWithForm } from "../utilities/RenderWithForm";
import { TestForm } from "../utilities/TestForm";
import RadioButtonGroup from "../../src/components/ui/RadioButtonGroup";

type SampleFormShape = {
    priority: string | undefined;
};

/** Test-only adapter to hook into RHF */
function RadioField({ name, options, rules }: { name: keyof SampleFormShape & string; options: { value: string; label: string }[]; rules?: Parameters<typeof Controller>[0]["rules"] }) {
    const { control } = useFormContext();
    return <Controller name={name} control={control} rules={rules} render={({ field }) => <RadioButtonGroup<SampleFormShape> name={name} options={options} selectedValue={field.value ?? ""} onChange={(e) => field.onChange(e.target.value)} />} />;
}

const OPTIONS = [
    { value: "low", label: "Low" },
    { value: "med", label: "Medium" },
    { value: "high", label: "High" },
];

describe("<RadioButtonGroup />", () => {
    it("Should render all options and none selected when value is undefined", () => {
        // Arrange
        renderWithForm<SampleFormShape>(<RadioField name="priority" options={OPTIONS} />, { defaultValues: { priority: undefined } });
        const low = screen.getByRole("radio", { name: /low/i });
        const med = screen.getByRole("radio", { name: /medium/i });
        const high = screen.getByRole("radio", { name: /high/i });

        // Assert
        expect(low).toBeInTheDocument();
        expect(med).toBeInTheDocument();
        expect(high).toBeInTheDocument();
        expect(low).not.toBeChecked();
        expect(med).not.toBeChecked();
        expect(high).not.toBeChecked();
    });

    it("Should bind value and updates RHF state when a radio is clicked", async () => {
        // Assert
        const user = userEvent.setup();
        const { methods } = renderWithForm<SampleFormShape>(<RadioField name="priority" options={OPTIONS} />, { defaultValues: { priority: undefined } });
        const med = screen.getByRole("radio", { name: /medium/i });

        // Act
        await user.click(med);

        // Assert
        expect(med).toBeChecked();
        expect(methods.getValues("priority")).toBe("med");
    });

    it("Should respect preset default value from defaultValues", () => {
        // Arrange
        renderWithForm<SampleFormShape>(<RadioField name="priority" options={OPTIONS} />, { defaultValues: { priority: "med" } });

        // Assert
        expect(screen.getByRole("radio", { name: /medium/i })).toBeChecked();
    });

    it("Should switche render appropriately", async () => {
        // Arrange
        renderWithForm<SampleFormShape>(<RadioField name="priority" options={OPTIONS} />, { defaultValues: { priority: "low" } });
        const low = screen.getByRole("radio", { name: /low/i });
        const high = screen.getByRole("radio", { name: /high/i });

        // Assert
        expect(low).toBeChecked();
        expect(high).not.toBeChecked();
    });

    
    it("Should switche selection when a different option is chosen", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods } = renderWithForm<SampleFormShape>(<RadioField name="priority" options={OPTIONS} />, { defaultValues: { priority: "low" } });
        const low = screen.getByRole("radio", { name: /low/i });
        const high = screen.getByRole("radio", { name: /high/i });

        // Act
        await user.click(high);

        // Assert
        expect(low).not.toBeChecked();
        expect(high).toBeChecked();
        expect(methods.getValues("priority")).toBe("high");
    });

    it("Should block submit when required and empty; allows submit after selection", async () => {
        // Arrange
        const user = userEvent.setup();
        const onSubmit = vi.fn();
        renderWithForm<SampleFormShape>(
            <TestForm onSubmit={onSubmit} submitLabel="Submit">
                <RadioField name="priority" options={OPTIONS} rules={{ required: "Priority is required" }} />
            </TestForm>,
            { defaultValues: { priority: undefined } }
        );

        // Act: No selection -> should not submit
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert
        expect(onSubmit).not.toHaveBeenCalled();

        // Act: After selecting, it should submit with value
        await user.click(screen.getByRole("radio", { name: /high/i }));
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert
        expect(onSubmit).toHaveBeenCalledWith(expect.objectContaining({ priority: "high" }), expect.anything());
    });

    it("Should keep controlled value as empty string when undefined (no radio checked)", () => {
        // Arrange
        renderWithForm<SampleFormShape>(<RadioField name="priority" options={OPTIONS} />, { defaultValues: { priority: undefined } });
        
        // Assert
        expect(screen.getByRole("radio", { name: /low/i })).not.toBeChecked();
        expect(screen.getByRole("radio", { name: /medium/i })).not.toBeChecked();
        expect(screen.getByRole("radio", { name: /high/i })).not.toBeChecked();
    });
});
