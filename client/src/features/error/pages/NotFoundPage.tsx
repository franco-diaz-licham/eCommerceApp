import { SearchOff } from "@mui/icons-material";
import { Box, Button, Typography } from "@mui/material";
import { Link } from "react-router-dom";

export default function NotFoundPage() {
    return (
        <Box
            sx={{
                minHeight: "calc(100vh - 300px)",
                display: "flex",
                flexDirection: "column",
                justifyContent: "center",
                alignItems: "center",
            }}
        >
            <SearchOff sx={{ fontSize: 100 }} color="primary" />
            <Typography gutterBottom variant="h3">
                Oops - we could not find what you were looking for
            </Typography>
            <Button component={Link} to="/products" size="large" variant="contained">
                Go back to shop
            </Button>
        </Box>
    );
}
