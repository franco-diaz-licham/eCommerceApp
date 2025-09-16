import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { renderWithForm } from "../utilities/RenderWithForm";
import { TestForm } from "../utilities/TestForm";
import { Field } from "../utilities/Field";

type FormShape = {
    title: string;
    qty: number | undefined;
    notes: string;
};

describe("<TextInput />", () => {
    it("should bind value and update form state when user types", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods } = renderWithForm<FormShape>(<Field name="title" label="Title" />, { defaultValues: { title: "", qty: undefined, notes: "" } });
        const input = screen.getByRole("textbox", { name: /title/i });

        // Act
        await user.type(input, "Hello World");

        // Assert
        expect(input).toHaveValue("Hello World");
        expect(methods.getValues("title")).toBe("Hello World");
    });

    it("should convert to a number when the user types digits", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods } = renderWithForm<FormShape>(<Field name="qty" label="Quantity" type="number" />, { defaultValues: { title: "", qty: undefined, notes: "" } });
        const spin = screen.getByRole("spinbutton", { name: /quantity/i });

        // Act
        await user.clear(spin);
        await user.type(spin, "42");

        // Assert
        expect(spin).toHaveValue(42);
        expect(methods.getValues("qty")).toBe(42);
    });

    it("should set undefined when the input is cleared", async () => {
        // Arrange (start with a non-empty value to prove it changes)
        const user = userEvent.setup();
        const { methods } = renderWithForm<FormShape>(<Field name="qty" label="Quantity" type="number" />, { defaultValues: { title: "", qty: 10, notes: "" } });
        const spin = screen.getByRole("spinbutton", { name: /quantity/i });

        // Act
        await user.clear(spin);

        // Assert
        expect(spin).toHaveValue(undefined);
        expect(methods.getValues("qty")).toBeUndefined();
    });

    it("should show helper text when submitted without a required value and hide after fixing", async () => {
        // Arrange
        const user = userEvent.setup();
        renderWithForm<FormShape>(
            <TestForm>
                <Field name="title" label="Title" rules={{ required: "Title is required" }} />
            </TestForm>,
            { defaultValues: { title: "", qty: undefined, notes: "" } }
        );

        // Act
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert
        expect(await screen.findByText(/title is required/i)).toBeInTheDocument();

        // Act (fix and resubmit)
        await user.type(screen.getByRole("textbox", { name: /title/i }), "Ok");
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert
        expect(screen.queryByText(/title is required/i)).not.toBeInTheDocument();
    });

    it("should render a textarea with the given rows when multiline is true", () => {
        // Arrange & Act
        renderWithForm<FormShape>(<Field name="notes" label="Notes" multiline rows={5} />);
        const area = screen.getByRole("textbox", { name: /notes/i });

        // Assert
        expect(area.tagName.toLowerCase()).toBe("textarea");
        expect(area).toHaveAttribute("rows", "5");
    });

    it("should render with the provided label when label prop is set", () => {
        // Arrange & Act
        renderWithForm<FormShape>(<Field name="title" label="My Label" />);
        const input = screen.getByRole("textbox", { name: /my label/i });

        // Assert
        expect(input).toBeInTheDocument();
        expect(input).toBeEnabled();
    });

    it("should render an empty string when value is undefined", () => {
        // Arrange (omit `title` in defaults to simulate undefined)
        renderWithForm<FormShape>(<Field name="title" label="Title" />, { defaultValues: { /* title omitted */ qty: undefined, notes: "" } });
        const input = screen.getByRole("textbox", { name: /title/i });

        // Assert
        expect(input).toHaveValue(""); // your component does value={field.value ?? ''}
    });

    it("should render the preset value when defaultValues provides it", () => {
        // Arrange
        renderWithForm<FormShape>(<Field name="title" label="Title" />, { defaultValues: { title: "Preset", qty: undefined, notes: "" } });

        // Assert
        const input = screen.getByRole("textbox", { name: /title/i });
        expect(input).toHaveValue("Preset");
    });
});
