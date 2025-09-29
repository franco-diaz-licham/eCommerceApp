import { screen } from "@testing-library/react";
import { render } from "@testing-library/react";
import Header from "../../src/components/ui/Header";
import { Button } from "@mui/material";

describe("<Header />", () => {
    it("Should render the given title text", () => {
        // Arrange
        render(<Header title="Dashboard" />);
        const heading = screen.getByRole("heading", { name: /dashboard/i });
        
        // Assert
        expect(heading).toBeInTheDocument();
        expect(heading.tagName.toLowerCase()).toBe("h4");
    });

    it("Should render children inside the right-hand Stack", () => {
        // Arrange
        render(
            <Header title="Settings">
                <Button>Save</Button>
            </Header>
        );

        // Asswert
        expect(screen.getByRole("button", { name: /save/i })).toBeInTheDocument();
    });

    it("Should renders the underline decoration below the title", () => {
        // Arrange
        render(<Header title="Reports" />);
        const underline = screen.getByTestId("underline");
        
        // Assert
        expect(underline).toBeInTheDocument();
    });
});
