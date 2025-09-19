import { useForm, useFormState } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Box, Grid, Paper, Typography } from "@mui/material";
import ImageIcon from "@mui/icons-material/Image";
import { useEffect, useRef } from "react";
import type { FileWithPreview, ProductFormData } from "../types/product.types";
import { createProductSchema, type CreateProductSchema } from "../types/createProductSchema";
import { useFetchFiltersQuery } from "../services/product.api";
import TextInput from "../../../components/ui/TextInput";
import SelectInput from "../../../components/ui/SelectInput";
import Dropzone from "../../../components/ui/Dropzone";
import { toProductFormData } from "../../../lib/mapper";
import ProductFormSkeleton from "./ProductFormSkeleton";

/** Functional props. */
type ProductFormProps = {
    product: ProductFormData | null;
    onFormCancel: () => void;
    onFormSubmit: (data: ProductFormData) => void;
    onDisabledChanged: (disabled: boolean) => void;
};

/** Reusable, nice-looking image placeholder */
function NoImagePlaceholder() {
    return (
        <Box
            sx={{
                width: "100%",
                maxWidth: 500,
                height: 600,
                border: "2px dashed",
                borderColor: "divider",
                borderRadius: 2,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                bgcolor: "background.default",
            }}
            aria-label="No image selected"
        >
            <Box sx={{ textAlign: "center", px: 3 }}>
                <ImageIcon sx={{ fontSize: 64, opacity: 0.4, mb: 1 }} />
                <Typography variant="body1" sx={{ opacity: 0.7, fontWeight: 600 }}>
                    No image selected
                </Typography>
                <Typography variant="body2" sx={{ opacity: 0.6 }}>
                    Drop a file or use the uploader to preview it here.
                </Typography>
            </Box>
        </Box>
    );
}

export default function ProductForm(props: ProductFormProps) {
    const methods = useForm<CreateProductSchema>({
        mode: "onChange",
        resolver: zodResolver(createProductSchema),
        defaultValues: {
            name: "",
            description: "",
            price: 100,
            type: undefined,
            brand: undefined,
            quantityInStock: 1,
            pictureUrl: undefined,
            photo: undefined,
        },
    });

    const { control, handleSubmit, watch, reset } = methods;
    const { isSubmitting, isValid } = useFormState({ control });
    const initialized = useRef(false);
    const watchFile = watch("photo") as FileWithPreview | undefined;
    const { data, isLoading } = useFetchFiltersQuery();

    useEffect(() => {
        props.onDisabledChanged(isSubmitting || !isValid);
    }, [isSubmitting, isValid, props]);

    // On edit, reset form with product values
    useEffect(() => {
        if (initialized.current) return;

        const p = props.product;
        const brandsReady = !!data?.brands?.length;
        const typesReady = !!data?.productTypes?.length;
        if (!p || !brandsReady || !typesReady) return;

        reset(
            {
                name: p.name,
                description: p.description,
                price: p.unitPrice,
                brand: p.brandId ?? undefined,
                type: p.productTypeId ?? undefined,
                quantityInStock: p.quantityInStock,
                pictureUrl: p.pictureUrl || undefined,
                photo: undefined,
            },
            { keepDirty: false }
        );

        initialized.current = true;
    }, [data, props.product, reset]);

    // Revoke preview URL
    useEffect(() => {
        const url = watchFile?.preview;
        return () => {
            if (url) URL.revokeObjectURL(url);
        };
    }, [watchFile?.preview]);

    const handleOnSubmit = (form: CreateProductSchema) => {
        props.onFormSubmit(toProductFormData(form, props.product?.id));
    };

    // --- Layout-accurate Skeleton while filters load ---
    if (isLoading || !data) return <ProductFormSkeleton />;

    // --- Form ready ---
    const previewSrc = watchFile?.preview || props.product?.pictureUrl;

    return (
        <Box component={Paper} sx={{ p: 4 }}>
            <form onSubmit={handleSubmit(handleOnSubmit)} noValidate id="submit-button">
                <Grid container spacing={3}>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <TextInput control={control} name="name" label="Product name" />
                        <SelectInput sx={{ mt: 3 }} items={data.brands.map((x) => ({ value: x.id, label: x.name }))} control={control} name="brand" label="Brand" />
                        <SelectInput sx={{ mt: 3 }} items={data.productTypes.map((x) => ({ value: x.id, label: x.name }))} control={control} name="type" label="Type" />
                        <TextInput sx={{ mt: 3 }} type="number" control={control} name="price" label="Price in cents" />
                        <TextInput sx={{ mt: 3 }} type="number" control={control} name="quantityInStock" label="Quantity in stock" />
                        <TextInput sx={{ mt: 3 }} control={control} multiline rows={4} name="description" label="Description" />
                        <Dropzone name="photo" control={control} />
                    </Grid>

                    <Grid size={{ xs: 12, md: 6 }} display={"flex"} justifyContent="center" alignItems="center">
                        {previewSrc ? <Box component="img" src={previewSrc} alt="product preview" sx={{ borderRadius: 5, maxHeight: 650, maxWidth: 500, objectFit: "cover" }} /> : <NoImagePlaceholder />}
                    </Grid>
                </Grid>
            </form>
        </Box>
    );
}
