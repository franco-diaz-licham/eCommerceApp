import { Box, Button, Divider, Paper, Typography } from "@mui/material";
import RadioButtonGroup from "../../../components/ui/RadioButtonGroup";
import CheckboxButtons from "../../../components/ui/CheckboxButtons";
import SearchField from "./SearchField";

/** Generic filter option dto. */
export type FilterOptionModel = {
    value: number;
    label: string;
};

/** Options for sorting products. */
const sortOptions = [
    { value: "nameAsc", label: "Alphabetical" },
    { value: "priceDesc", label: "Price: High to low" },
    { value: "priceAsc", label: "Price: Low to high" },
];

export type FiltersProps = {
    brandOptions: FilterOptionModel[];
    typeOptions: FilterOptionModel[];
    selectedBrandIds: number[];
    selectedTypeIds: number[];
    orderBy: string | null | undefined;
    searchTerm: string;

    onOrderByChange: (value: string) => void;
    onBrandsChange: (ids: number[]) => void;
    onTypesChange: (ids: number[]) => void;
    onSearchChange: (value: string) => void;
    onReset: () => void;
};

export default function Filters({ brandOptions, typeOptions, selectedBrandIds, selectedTypeIds, orderBy, searchTerm, onOrderByChange, onBrandsChange, onTypesChange, onSearchChange, onReset }: FiltersProps) {
    const checkedBrands = brandOptions.filter((x) => selectedBrandIds.includes(x.value));
    const checkedTypes = typeOptions.filter((x) => selectedTypeIds.includes(x.value));

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
                <SearchField value={searchTerm} onSearchChange={onSearchChange} />
            </Paper>

            <Paper sx={{ p: 1 }} elevation={0}>
                <RadioButtonGroup name="order-by" selectedValue={orderBy ?? ""} options={sortOptions} onChange={(e) => onOrderByChange(e.target.value)} />
            </Paper>

            <Divider />

            <Paper sx={{ p: 1 }} elevation={0}>
                <CheckboxButtons name="brands" items={brandOptions} checked={checkedBrands} onChange={(items) => onBrandsChange(items.map((x) => x.value))} />
            </Paper>

            <Divider />

            <Paper sx={{ p: 1 }} elevation={0}>
                <CheckboxButtons name="types" items={typeOptions} checked={checkedTypes} onChange={(items) => onTypesChange(items.map((x) => x.value))} />
            </Paper>

            <Button onClick={onReset}>Reset filters</Button>
        </Box>
    );
}
