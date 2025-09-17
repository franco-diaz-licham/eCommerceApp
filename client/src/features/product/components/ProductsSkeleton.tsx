import { Grid, Paper, Skeleton, Stack } from "@mui/material";

export default function ProductSkeleton() {
    return (
        <Grid container spacing={{ xs: 2, md: 4 }} sx={{ pt: 4 }}>
            {/* Sidebar skeleton */}
            <Grid size={{ xs: 12, md: 4, lg: 3 }}>
                <Paper
                    elevation={3}
                    variant="outlined"
                    sx={{
                        p: { xs: 2, md: 2.5 },
                        borderRadius: 3,
                        position: { md: "sticky" },
                        top: { md: 96 },
                        height: 1250,
                    }}
                >
                    <Stack spacing={2}>
                        <Skeleton variant="text" width="60%" height={32} />
                        {[...Array(24)].map((_, i) => (
                            <Skeleton key={i} variant="rectangular" height={32} />
                        ))}
                    </Stack>
                </Paper>
            </Grid>

            {/* Product grid skeleton */}
            <Grid size={{ xs: 12, md: 8, lg: 9 }}>
                <Grid container spacing={4}>
                    {[...Array(12)].map((_, i) => (
                        <Grid key={i} size={{ xs: 12, sm: 6, md: 6, lg: 4, xl: 3 }}>
                            <Paper
                                variant="outlined"
                                sx={{
                                    borderRadius: 3,
                                    overflow: "hidden",
                                    p: 1.5,
                                }}
                            >
                                <Skeleton variant="rectangular" height={230} />
                                <Skeleton variant="text" width="80%" sx={{ mt: 2 }} />
                                <Skeleton variant="text" width="60%" sx={{ mb: 4 }} />
                                <Skeleton variant="rectangular" height={32} sx={{ mt: 1 }} />
                            </Paper>
                        </Grid>
                    ))}
                </Grid>
            </Grid>
        </Grid>
    );
}
