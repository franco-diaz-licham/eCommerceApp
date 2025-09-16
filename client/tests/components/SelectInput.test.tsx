import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { renderWithForm } from "../utilities/RenderWithForm";
import { TestForm } from "../utilities/TestForm";
import SelectInput from "../../src/components/ui/SelectInput";

type FormShape = { option?: number };

const schema = z.object({
    option: z.number({ error: "Required" }),
});

const items = [
    { value: 1, label: "One" },
    { value: 2, label: "Two" },
    { value: 3, label: "Three" },
];

describe("<SelectInput /> ", () => {
    test("initial RHF state is undefined and placeholder is visible", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods } = renderWithForm<FormShape>(
            <TestForm submitLabel="Submit">
                <SelectInput<FormShape> name="option" label="Option" items={items} />
            </TestForm>,
            {
                defaultValues: { option: undefined },
                resolver: zodResolver(schema),
            }
        );
        const select = await screen.findByRole("combobox", { name: /option/i });

        // Act: click on select
        await user.click(select);

        // Assert
        expect(select).toBeInTheDocument();
        expect(methods.getValues().option).toBeUndefined();
        expect(await screen.findByRole("option", { name: /select option/i })).toBeInTheDocument();
    });

    test("changing the select updates RHF state (number) and submits correctly", async () => {
        // Arrange
        const user = userEvent.setup();
        const onSubmit = vi.fn();
        const { methods } = renderWithForm<FormShape>(
            <TestForm onSubmit={onSubmit} submitLabel="Submit">
                <SelectInput<FormShape> name="option" label="Option" items={items} />
            </TestForm>,
            {
                defaultValues: { option: undefined },
                resolver: zodResolver(schema),
            }
        );

        // Act: open, select and submit
        await user.click(screen.getByLabelText("Option"));
        await user.click(await screen.findByRole("option", { name: "Two" }));
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert
        expect(methods.getValues().option).toBe(2);
        expect(onSubmit).toHaveBeenCalledWith(expect.objectContaining({ option: 2 }), expect.anything());
    });

    test("shows validation error when empty on submit", async () => {
        // Arrange
        const user = userEvent.setup();
        const onSubmit = vi.fn();
        renderWithForm<FormShape>(
            <TestForm onSubmit={onSubmit} submitLabel="Submit">
                <SelectInput<FormShape> name="option" label="Option" items={items} />
            </TestForm>,
            {
                defaultValues: { option: undefined },
                resolver: zodResolver(schema),
            }
        );

        // Act: No selection made on submit
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert: Error is shown from RHF/Zod
        expect(await screen.findByText("Required")).toBeInTheDocument();
        expect(onSubmit).not.toHaveBeenCalled();
    });

    test("clearing selection sets RHF value back to undefined", async () => {
        // Arrange
        const user = userEvent.setup();
        const onSubmit = vi.fn();
        const { methods } = renderWithForm<FormShape>(
            <TestForm onSubmit={onSubmit} submitLabel="Save">
                <SelectInput<FormShape> name="option" label="Option" items={items} />
            </TestForm>,
            {
                defaultValues: { option: 1 }, // start selected
                resolver: zodResolver(schema),
            }
        );

        // Act: Open the select and choose the empty item
        await user.click(screen.getByLabelText("Option"));
        await user.click(await screen.findByRole("option", { name: "Select Option" }));

        // Assert
        expect(methods.getValues().option).toBeUndefined();
    });
});
