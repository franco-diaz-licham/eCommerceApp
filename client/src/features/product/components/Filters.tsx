import { Box, Button, Divider, Paper, Typography } from "@mui/material";
import { useAppDispatch, useAppSelector } from "../../../app/store/store";
import RadioButtonGroup from "../../../components/ui/RadioButtonGroup";
import CheckboxButtons from "../../../components/ui/CheckboxButtons";
import { resetParams, setBrands, setOrderBy, setTypes } from "../services/productSlice";
import type { BrandResponse } from "../../../entities/brand/brand.types";
import type { ProductTypeResponse } from "../../../entities/productType/productTypeResponse.type";
import SearchField from "./SearchField";

const sortOptions = [
    { value: "nameAsc", label: "Alphabetical" },
    { value: "priceDesc", label: "Price: High to low" },
    { value: "priceAsc", label: "Price: Low to high" },
];

type FiltersProps = {
    filtersData: { brands: BrandResponse[]; types: ProductTypeResponse[] };
};

export default function Filters(props: FiltersProps) {
    const { orderBy, productTypeIds, brandIds } = useAppSelector((state) => state.products);
    const dispatch = useAppDispatch();
    const allBrands = props.filtersData.brands.map((x) => ({ value: x.id, label: x.name }));
    const allTypes = props.filtersData.types.map((x) => ({ value: x.id, label: x.name }));
    const checkedBrands = props.filtersData.brands.filter((x) => brandIds?.some((id) => id === x.id)).map((x) => ({ value: x.id, label: x.name }));
    const checkedTypes = props.filtersData.types.filter((x) => productTypeIds?.some((id) => id === x.id)).map((x) => ({ value: x.id, label: x.name }));

    return (
        <Box display="flex" flexDirection="column" gap={1}>
            <Box sx={{ display: "inline-block", position: "relative", mb: 2 }}>
                <Typography variant="h6" sx={{ fontWeight: 800 }}>
                    Filters
                </Typography>
                <Box
                    sx={{
                        position: "absolute",
                        bottom: -4,
                        left: 0,
                        width: 32,
                        height: 3,
                        bgcolor: "primary.main",
                        borderRadius: 2,
                    }}
                />
            </Box>
            <Paper sx={{ p: 0 }} elevation={0}>
                <SearchField />
            </Paper>
            <Paper sx={{ p: 1 }} elevation={0}>
                <RadioButtonGroup selectedValue={orderBy ?? ""} options={sortOptions} onChange={(e) => dispatch(setOrderBy(e.target.value))} name="order-by" />
            </Paper>
            <Divider />
            <Paper sx={{ p: 1 }} elevation={0}>
                <CheckboxButtons items={allBrands} checked={checkedBrands} onChange={(items) => dispatch(setBrands(items.map((x) => x.value)))} name="brands" />
            </Paper>
            <Divider />
            <Paper sx={{ p: 1 }} elevation={0}>
                <CheckboxButtons items={allTypes} checked={checkedTypes} onChange={(items) => dispatch(setTypes(items.map((x) => x.value)))} name="types" />
            </Paper>
            <Button onClick={() => dispatch(resetParams())}>Reset filters</Button>
        </Box>
    );
}
