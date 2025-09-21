import { Paper, Box, Alert, Grid } from "@mui/material";
import ProductList from "../components/ProductList";
import { useFetchFiltersQuery, useFetchProductsQuery } from "../api/product.api";
import { useAppDispatch, useAppSelector } from "../../../app/store/store";
import Filters, { type FilterOptionDto } from "../components/Filters";
import AppPagination from "../../../components/ui/AppPagination";
import { resetParams, setBrands, setOrderBy, setTypes, setPageNumber, setSearchTerm } from "../api/productSlice";
import ProductSkeleton from "../components/ProductsSkeleton";
import { useBasket } from "../../../hooks/useBasket";
import type { ProductResponse } from "../models/product.types";
import type { ProductCardDto } from "../components/ProductCard";

export default function ProductsPage() {
    const params = useAppSelector((state) => state.products);
    const { data, isLoading: isProductsLoading } = useFetchProductsQuery(params);
    const { data: filtersData, isLoading: isFiltersLoading } = useFetchFiltersQuery();
    const dispatch = useAppDispatch();

    // basket I/O stays in the page (not in cards)
    const { isAdding, addItemEnsuringBasket } = useBasket();
    const handleAddToCart = (productId: number) => addItemEnsuringBasket(productId, 1);

    if (isProductsLoading || isFiltersLoading) return <ProductSkeleton />;
    if (!data || !filtersData) return <Alert severity="error">Failed to load products or filters.</Alert>;

    // map API → OptionDto
    const brandOptions: FilterOptionDto[] = filtersData.brands.map((b) => ({ value: b.id, label: b.name }));
    const typeOptions: FilterOptionDto[] = filtersData.productTypes.map((t) => ({ value: t.id, label: t.name }));

    // map API products → ProductCardDto for the dumb list/cards
    const toCardDto = (p: ProductResponse): ProductCardDto => ({
        id: p.id,
        name: p.name,
        unitPrice: p.unitPrice,
        photoUrl: p.photo?.publicUrl,
    });
    const cardProducts: ProductCardDto[] = (data.response ?? []).map(toCardDto);

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
                        brandOptions={brandOptions}
                        typeOptions={typeOptions}
                        selectedBrandIds={params.brandIds ?? []}
                        selectedTypeIds={params.productTypeIds ?? []}
                        orderBy={params.orderBy}
                        searchTerm={params.searchTerm ?? ""}
                        onOrderByChange={(value) => dispatch(setOrderBy(value))}
                        onBrandsChange={(ids) => dispatch(setBrands(ids))}
                        onTypesChange={(ids) => dispatch(setTypes(ids))}
                        onSearchChange={(value) => dispatch(setSearchTerm(value))}
                        onReset={() => dispatch(resetParams())}
                    />
                </Paper>
            </Grid>

            {/* Product grid + pagination */}
            <Grid size={{ xs: 12, md: 8, lg: 9 }}>
                {cardProducts.length ? (
                    <>
                        <ProductList products={cardProducts} isAdding={isAdding} onAddToCart={handleAddToCart} />
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
