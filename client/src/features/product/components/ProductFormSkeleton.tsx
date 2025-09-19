import { Box, Grid, Paper, Skeleton } from "@mui/material";

export default function ProductFormSkeleton() {
    return (
        <Box component={Paper} sx={{ p: 4 }}>
            <Grid container spacing={3}>
                {/* Form fields */}
                <Grid size={{ xs: 12, md: 6 }}>
                    <Skeleton height={56} sx={{ mb: 3 }} animation="wave" />
                    <Skeleton height={56} sx={{ mb: 3 }} animation="wave" />
                    <Skeleton height={56} sx={{ mb: 3 }} animation="wave" />
                    <Skeleton height={56} sx={{ mb: 3 }} animation="wave" />
                    <Skeleton height={56} sx={{ mb: 3 }} animation="wave" />
                    <Skeleton variant="rectangular" height={120} sx={{ mb: 3, borderRadius: 1 }} animation="wave" />
                    <Skeleton variant="rectangular" height={120} sx={{ borderRadius: 1 }} animation="wave" />
                </Grid>

                {/* Image preview */}
                <Grid size={{ xs: 12, md: 6 }} display="flex" justifyContent="center" alignItems="center">
                    <Box sx={{ width: "100%" }}>
                        <Skeleton variant="rectangular" height={650} sx={{ borderRadius: 2 }} animation="wave" />
                    </Box>
                </Grid>
            </Grid>
        </Box>
    );
}
