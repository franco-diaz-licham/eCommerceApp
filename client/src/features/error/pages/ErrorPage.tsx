import { Button, ButtonGroup, Container, Typography } from "@mui/material";
import { useLazyGet400ErrorQuery, useLazyGet401ErrorQuery, useLazyGet404ErrorQuery, useLazyGet500ErrorQuery, usePostValidationErrorMutation } from "../api/error.api";
import { getErrorMessage } from "../../../lib/utils";
import { toast } from "react-toastify";

export default function ErrorPage() {
    const [trigger400Error] = useLazyGet400ErrorQuery();
    const [trigger401Error] = useLazyGet401ErrorQuery();
    const [trigger404Error] = useLazyGet404ErrorQuery();
    const [trigger500Error] = useLazyGet500ErrorQuery();
    const [triggerValidationError] = usePostValidationErrorMutation();

    const getError = async (data: "bad-request" | "validation-error") => {
        try {
            if (data === "bad-request") await trigger400Error().unwrap();
            else await triggerValidationError().unwrap();
        } catch (e: unknown) {
            toast.error(getErrorMessage(e, "There was an error"));
        }
    };

    return (
        <Container maxWidth="lg">
            <Typography gutterBottom variant="h3">
                Errors for testing
            </Typography>
            <ButtonGroup fullWidth>
                <Button variant="contained" onClick={() => getError("bad-request")}>
                    Test 400 Error
                </Button>
                <Button variant="contained" onClick={() => trigger401Error().catch((err) => console.log(err))}>
                    Test 401 Error
                </Button>
                <Button variant="contained" onClick={() => trigger404Error().catch((err) => console.log(err))}>
                    Test 404 Error
                </Button>
                <Button variant="contained" onClick={() => trigger500Error().catch((err) => console.log(err))}>
                    Test 500 Error
                </Button>
                <Button variant="contained" onClick={() => getError("validation-error")}>
                    Test Validation Error
                </Button>
            </ButtonGroup>
        </Container>
    );
}
