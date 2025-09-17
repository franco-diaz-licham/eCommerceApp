import { Button, Container, Grid, Paper, Stack, TextField, Typography } from "@mui/material";

export default function NewsLetter() {
    return (
        <Container maxWidth="lg">
            <Paper variant="outlined" sx={{ p: { xs: 3, md: 5 }, borderRadius: 3 }}>
                <Grid container spacing={3} alignItems="center">
                    <Grid>
                        <Typography variant="h6" fontWeight={800}>
                            Get 10% off your first order
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            Join our newsletter for exclusive drops and early access.
                        </Typography>
                    </Grid>
                    <Grid>
                        <Stack direction={{ xs: "column", sm: "row" }} spacing={1.5}>
                            <TextField fullWidth placeholder="Enter your email" />
                            <Button variant="contained">Subscribe</Button>
                        </Stack>
                    </Grid>
                </Grid>
            </Paper>
        </Container>
    );
}
