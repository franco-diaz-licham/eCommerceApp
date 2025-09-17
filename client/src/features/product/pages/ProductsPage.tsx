import { Paper, Box, Alert, Grid } from "@mui/material";
import ProductList from "../components/ProductList";
import { useFetchFiltersQuery, useFetchProductsQuery } from "../services/product.api";
import { useAppDispatch, useAppSelector } from "../../../app/store/store";
import Filters from "../components/Filters";
import AppPagination from "../../../components/ui/AppPagination";
import { setPageNumber } from "../services/productSlice";
import ProductSkeleton from "../components/ProductsSkeleton";

export default function ProductsPage() {
    const params = useAppSelector((state) => state.products);
    const { data, isLoading: isProductsLoading } = useFetchProductsQuery(params);
    const { data: filtersData, isLoading: isFiltersLoading } = useFetchFiltersQuery();
    const dispatch = useAppDispatch();

    // Loading skeleton
    if (isProductsLoading || isFiltersLoading) return <ProductSkeleton />;

    // Error loading products
    if (!data || !filtersData) return <Alert severity="error">Failed to load products or filters.</Alert>;

    // Products content
    return (
        <Grid container spacing={{ xs: 2, md: 4 }} sx={{ pt: 4 }}>
            {/* Sidebar */}
            <Grid size={{ xs: 12, md: 4, lg: 3 }}>
                <Paper
                    elevation={3}
                    variant="outlined"
                    sx={{
                        p: { xs: 2, md: 2.5 },
                        borderRadius: 3,
                        position: { md: "sticky" },
                        top: { md: 96 },
                        boxShadow: 3,
                    }}
                >
                    <Filters
                        filtersData={{
                            brands: filtersData.brands,
                            types: filtersData.productTypes,
                        }}
                    />
                </Paper>
            </Grid>

            {/* Product grid + pagination */}
            <Grid size={{ xs: 12, md: 8, lg: 9 }}>
                {data.response?.length ? (
                    <>
                        <ProductList products={data.response} />
                        <Box
                            sx={{
                                mt: { xs: 3, md: 4 },
                                display: "flex",
                                justifyContent: "center",
                            }}
                        >
                            <AppPagination
                                metadata={data.pagination}
                                onPageChange={(page: number) => {
                                    dispatch(setPageNumber(page));
                                    window.scrollTo({ top: 0, behavior: "smooth" });
                                }}
                            />
                        </Box>
                    </>
                ) : (
                    <Alert severity="info">There are no results for this filter.</Alert>
                )}
            </Grid>
        </Grid>
    );
}
