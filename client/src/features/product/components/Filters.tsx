import { Box, Button, Paper } from "@mui/material";
import { useAppDispatch, useAppSelector } from "../../../app/store/store";
import { Search } from "@mui/icons-material";
import RadioButtonGroup from "../../../components/ui/RadioButtonGroup";
import CheckboxButtons from "../../../components/ui/CheckboxButtons";
import { resetParams, setBrands, setOrderBy, setTypes } from "../services/productSlice";
import type { BrandResponse } from "../../../entities/brand/brand.types";
import type { ProductResponse } from "../types/product.types";

const sortOptions = [
    { value: "name", label: "Alphabetical" },
    { value: "priceDesc", label: "Price: High to low" },
    { value: "price", label: "Price: Low to high" },
];

type Props = {
    filtersData: { brands: BrandResponse[]; types: ProductResponse[] };
};

export default function Filters({ filtersData: data }: Props) {
    const { orderBy, types, brands } = useAppSelector((state) => state.products);
    const dispatch = useAppDispatch();

    return (
        <Box display="flex" flexDirection="column" gap={3}>
            <Paper>
                <Search />
            </Paper>
            <Paper sx={{ p: 3 }}>
                <RadioButtonGroup selectedValue={orderBy ?? ""} options={sortOptions} onChange={(e) => dispatch(setOrderBy(e.target.value))} />
            </Paper>
            <Paper sx={{ p: 3 }}>
                <CheckboxButtons items={data.brands.map((x) => x.name)} checked={data.brands.filter((x) => brands?.some((value) => value === x.id)).map((x) => x.name)} onChange={(items: string[]) => dispatch(setBrands(items))} />
            </Paper>
            <Paper sx={{ p: 3 }}>
                <CheckboxButtons items={data.types.map((x) => x.name)} checked={data.types.filter((x) => types?.some((value) => value === x.id)).map((x) => x.name)} onChange={(items: string[]) => dispatch(setTypes(items))} />
            </Paper>
            <Button onClick={() => dispatch(resetParams())}>Reset filters</Button>
        </Box>
    );
}
