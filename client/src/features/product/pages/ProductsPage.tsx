import { Grid, Typography } from "@mui/material";
import ProductList from "../components/ProductList";
import { useFetchFiltersQuery, useFetchProductsQuery } from "../services/product.api";
import { useAppDispatch, useAppSelector } from "../../../app/store/store";
import Filters from "../components/Filters";
import AppPagination from "../../../components/ui/AppPagination";
import { setPageNumber } from "../services/productSlice";

export default function ProductsPage() {
    const params = useAppSelector((state) => state.products);
    const { data, isLoading } = useFetchProductsQuery(params);
    const { data: filtersData, isLoading: filtersLoading } = useFetchFiltersQuery();
    const dispatch = useAppDispatch();

    if (isLoading || !data || filtersLoading || !filtersData) return <div>Loading...</div>;

    return (
        <>
            <Typography variant="h4" sx={{ fontWeight: "bold" }}>
                Products
            </Typography>
            <Grid container spacing={4}>
                <Grid size={3}>
                    <Filters filtersData={{ brands: filtersData.brands, types: filtersData.productTypes }} />
                </Grid>
                <Grid size={9}>
                    {data.response && data.response.length > 0 ? (
                        <>
                            <ProductList products={data.response} />
                            <AppPagination
                                metadata={data.pagination}
                                onPageChange={(page: number) => {
                                    dispatch(setPageNumber(page));
                                    window.scrollTo({ top: 0, behavior: "smooth" });
                                }}
                            />
                        </>
                    ) : (
                        <Typography variant="h5">There are no results for this filter</Typography>
                    )}
                </Grid>
            </Grid>
        </>
    );
}
