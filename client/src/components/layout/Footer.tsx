import { Box, Container, Link, Stack, Typography } from "@mui/material";
import DarkLogo from "../../assets/dark-logo.png";
import WhiteLogo from "../../assets/white-logo.png";
import { useAppSelector } from "../../app/store/store";

export default function Footer() {
    const { darkMode } = useAppSelector((state) => state.ui);
    return (
        <Box sx={{ bgcolor: "background.paper", borderTop: (t) => `1px solid ${t.palette.divider}` }}>
            <Container maxWidth="xl" sx={{ py: 4 }}>
                <Stack direction="row" justifyContent="space-between" alignItems="center" >
                    <Stack direction={"column"} display={"flex"} alignItems={"center"}>
                        <Box
                            component="img"
                            src={darkMode ? WhiteLogo : DarkLogo}
                            alt="Logo"
                            sx={{
                                height: 30,
                                width: 120,
                            }}
                        />
                        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                            © {new Date().getFullYear()} eGo Store. All rights reserved.
                        </Typography>
                    </Stack>
                    <Stack direction="row" spacing={3} justifyContent={{ xs: "flex-start", md: "flex-end" }}>
                        <Link href="#" color="text.secondary" underline="hover">
                            Privacy
                        </Link>
                        <Link href="#" color="text.secondary" underline="hover">
                            Terms
                        </Link>
                        <Link href="#" color="text.secondary" underline="hover">
                            Support
                        </Link>
                    </Stack>
                </Stack>
            </Container>
        </Box>
    );
}
