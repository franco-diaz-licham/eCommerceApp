import { Box, Button, Container, Grid, Stack, Typography } from "@mui/material";
import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";
import RestartAltIcon from "@mui/icons-material/RestartAlt";
import SupportAgentIcon from "@mui/icons-material/SupportAgent";
import { ValueProp } from "./ValueProp";

export default function Hero() {
    return (
        <Box
            sx={(t) => ({
                background: `linear-gradient(135deg, ${t.palette.primary.light} 0%, ${t.palette.info.light} 45%, ${t.palette.background.default} 100%)`,
                py: { xs: 10, md: 16, lg: 22, xl: 28 },
                borderBottom: `1px solid ${t.palette.divider}`,
            })}
        >
            <Container maxWidth="lg">
                <Grid container spacing={6} alignItems="center">
                    <Grid>
                        <Typography variant="h2" fontWeight={800} gutterBottom>
                            Bring your style to life
                        </Typography>
                        <Typography variant="h6" color="text.secondary" sx={{ mb: 3 }}>
                            Discover curated essentials for every season. Quality pieces, sharp prices, fast delivery.
                        </Typography>
                        <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                            <Button variant="contained" size="large" endIcon={<ArrowForwardIcon />}>
                                Shop new in
                            </Button>
                            <Button variant="outlined" size="large">
                                Explore all products
                            </Button>
                        </Stack>

                        <Stack direction={{ xs: "column", sm: "row" }} spacing={3} sx={{ mt: 5 }}>
                            <ValueProp icon={<LocalShippingIcon />} title="Free Shipping" subtitle="On orders over $75" />
                            <ValueProp icon={<RestartAltIcon />} title="Free Returns" subtitle="30-day policy" />
                            <ValueProp icon={<SupportAgentIcon />} title="Support" subtitle="7 days a week" />
                        </Stack>
                    </Grid>
                </Grid>
            </Container>
        </Box>
    );
}
