import { Box, Paper, Stack, Typography } from "@mui/material";
import type { ReactNode } from "react";

interface HeaderProps {
    title: string;
    children?: ReactNode;
}

export default function Header(props: HeaderProps) {
    return (
        <Box
            component={Paper}
            sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                flexWrap: "wrap",
                mb: 2,
                py: 1,
                px: 2,
            }}
        >
            <Box sx={{ display: "inline-block", position: "relative", mb: 2 }}>
                <Typography variant="h4" sx={{ fontWeight: 800 }}>
                    {props.title}
                </Typography>
                <Box
                    sx={{
                        position: "absolute",
                        bottom: -4,
                        left: 0,
                        width: 32,
                        height: 3,
                        bgcolor: "primary.main",
                        borderRadius: 2,
                    }}
                />
            </Box>
            <Stack direction={"row"}>{props.children}</Stack>
        </Box>
    );
}
