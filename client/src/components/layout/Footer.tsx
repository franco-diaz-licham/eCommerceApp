import { Box, Container, Grid, Link, Stack, Typography } from "@mui/material";
import DarkLogo from "../../assets/dark-logo.png";
import WhiteLogo from "../../assets/white-logo.png";
import { useAppSelector } from "../../app/store/store";

export default function Footer() {
    const { darkMode } = useAppSelector((state) => state.ui);
    return (
        <Box sx={{ bgcolor: "background.paper", borderTop: (t) => `1px solid ${t.palette.divider}` }}>
            <Container maxWidth="xl" sx={{ py: 4 }}>
                <Grid container>
                    <Grid size={{ xs: 12, sm: 6 }} display={"flex"} justifyContent={{ xs: "center", md: "flex-start" }}>
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
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <Stack direction="row" spacing={3} justifyContent={{ xs: "center", md: "flex-end" }}>
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
                    </Grid>
                </Grid>
            </Container>
        </Box>
    );
}
