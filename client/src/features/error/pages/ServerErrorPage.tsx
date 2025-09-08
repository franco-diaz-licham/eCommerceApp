import { Divider, Paper, Typography } from "@mui/material";
import { useLocation } from "react-router-dom";
import type { ApiResponseError } from "../../../types/api.types";

export default function ServerErrorPage() {
    const { state } = useLocation();
    const error: ApiResponseError = state.error;

    return (
        <Paper>
            {state.error ? (
                <>
                    <Typography gutterBottom variant="h5" sx={{ px: 4, pt: 2 }} color="secondary">
                        {error.message} - {error.statusCode}
                    </Typography>
                    <Divider />
                    <Typography variant="body1" sx={{ p: 4 }}>
                        {error.details}
                    </Typography>
                </>
            ) : (
                <Typography variant="h5" gutterBottom>
                    Server error
                </Typography>
            )}
        </Paper>
    );
}
