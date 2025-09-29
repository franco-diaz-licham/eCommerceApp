import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { renderWithForm } from "../utilities/RenderWithForm";
import { TestForm } from "../utilities/TestForm";
import Dropzone from "../../src/components/ui/Dropzone";

type SampleFormShape = {
    photo?: File & { preview?: string };
};

describe("<Dropzone />", () => {
    // JSDOM doesn't implement this, so mock it for preview creation
    const originalCreateObjectURL = URL.createObjectURL;

    beforeEach(() => {
        // return a stable string we can assert against
        URL.createObjectURL = vi.fn(() => "blob:test-preview");
    });

    afterEach(() => {
        URL.createObjectURL = originalCreateObjectURL;
        vi.clearAllMocks();
    });

    it("Should bind uploaded file to RHF state and creates a preview URL", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods, container } = renderWithForm<SampleFormShape>(<Dropzone<SampleFormShape> name="photo" />);
        // find the file input the dropzone renders
        const input = container.querySelector('input[type="file"]') as HTMLInputElement;
        expect(input).toBeInTheDocument();
        const file = new File(["hello"], "avatar.png", { type: "image/png" });

        // Act
        await user.upload(input, file);

        // Assert
        const stored = methods.getValues("photo");
        expect(stored).toBeInstanceOf(File);
        expect(stored?.name).toBe("avatar.png");
        expect(stored?.preview).toBe("blob:test-preview");
        expect(URL.createObjectURL).toHaveBeenCalledWith(file);
    });

    it("Should show helper text when required; then submits after upload", async () => {
        // Arrange
        const user = userEvent.setup();
        const onSubmit = vi.fn();
        renderWithForm<SampleFormShape>(
            <TestForm onSubmit={onSubmit}>
                <Dropzone<SampleFormShape> name="photo" rules={{ required: "Image is required" }} />
            </TestForm>,
            { defaultValues: { photo: undefined } }
        );

        // Act
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert
        expect(await screen.findByText(/image is required/i)).toBeInTheDocument();
        expect(onSubmit).not.toHaveBeenCalled();
    });

    
    it("Should show helper text when submitted empty; then submits after upload", async () => {
        // Arrange
        const user = userEvent.setup();
        const onSubmit = vi.fn();
        const { container } = renderWithForm<SampleFormShape>(
            <TestForm onSubmit={onSubmit}>
                <Dropzone<SampleFormShape> name="photo" rules={{ required: "Image is required" }} />
            </TestForm>,
            { defaultValues: { photo: undefined } }
        );
        const input = container.querySelector('input[type="file"]') as HTMLInputElement;

        // Act
        await user.upload(input, new File(["x"], "x.jpg", { type: "image/jpeg" }));
        await user.click(screen.getByRole("button", { name: /submit/i }));

        // Assert
        expect(onSubmit).toHaveBeenCalledWith(expect.objectContaining({ photo: expect.any(File) }), expect.anything());
    });

    it("Should use only the first file when multiple files are provided", async () => {
        // Arrange
        const user = userEvent.setup();
        const { methods, container } = renderWithForm<SampleFormShape>(<Dropzone<SampleFormShape> name="photo" />);
        const input = container.querySelector('input[type="file"]') as HTMLInputElement;
        const file1 = new File(["a"], "first.png", { type: "image/png" });
        const file2 = new File(["b"], "second.png", { type: "image/png" });

        // Act
        await user.upload(input, [file1, file2]);

        // Assert
        const stored = methods.getValues("photo");
        expect(stored?.name).toBe("first.png");
    });
});
