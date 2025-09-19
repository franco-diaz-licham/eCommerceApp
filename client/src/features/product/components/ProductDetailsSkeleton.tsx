import { Grid, Paper, Skeleton, Stack } from "@mui/material";

export default function ProductDetailsSkeleton() {
    return (
        <Grid container spacing={{ xs: 3, md: 6 }} sx={{ mx: "auto", pt: 4 }}>
            <Grid size={{ xs: 12, md: 6 }}>
                <Paper variant="outlined" sx={{ borderRadius: 3, overflow: "hidden" }}>
                    <Skeleton variant="rectangular" sx={{ width: "100%", height: { xs: 360, md: 800, xl: 800 } }} animation="wave" />
                </Paper>
            </Grid>

            <Grid size={{ xs: 12, md: 6 }}>
                <Skeleton width="70%" height={48} sx={{ mb: 3 }} animation="wave" />
                <Skeleton width={160} height={48} sx={{ mb: 3 }} animation="wave" />
                <Stack spacing={1}>
                    <Skeleton height={30} width="90%" animation="wave" />
                    <Skeleton height={30} width="80%" animation="wave" />
                    <Skeleton height={30} width="75%" animation="wave" />
                    <Skeleton height={30} width="80%" animation="wave" />
                    <Skeleton height={30} width="80%" animation="wave" />
                </Stack>
                <Grid container spacing={2} sx={{ mt: 3 }}>
                    <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
                        <Skeleton variant="rectangular" height={56} animation="wave" />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <Skeleton variant="rectangular" height={56} animation="wave" />
                    </Grid>
                </Grid>
            </Grid>
        </Grid>
    );
}
