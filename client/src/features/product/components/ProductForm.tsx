import { useForm, useFormState, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Box, Button, Grid, Paper, Skeleton, Typography } from "@mui/material";
import ImageIcon from "@mui/icons-material/Image";
import { useEffect, useRef } from "react";
import type { FileWithPreview, ProductFormData } from "../types/product.types";
import { createProductSchema, type CreateProductSchema } from "../types/createProductSchema";
import { useFetchFiltersQuery } from "../services/product.api";
import TextInput from "../../../components/ui/TextInput";
import SelectInput from "../../../components/ui/SelectInput";
import Dropzone from "../../../components/ui/Dropzone";
import { toProductFormData } from "../../../lib/mapper";

/** Functional props. */
type ProductFormProps = {
    product: ProductFormData | null;
    onFormCancel: () => void;
    onFormSubmit: (data: ProductFormData) => void;
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
    if (isLoading || !data) {
        return (
            <Box sx={{ pt: 4 }}>
                <Box component={Paper} sx={{ p: 4, maxWidth: "lg", mx: "auto" }}>
                    {/* Heading + actions (skeleton) */}
                    <Box
                        sx={{
                            display: "flex",
                            justifyContent: "space-between",
                            alignItems: "center",
                            mb: 3,
                        }}
                    >
                        <Box sx={{ display: "inline-block", position: "relative", mb: 2 }}>
                            <Skeleton width={260} height={40} />
                            <Box
                                sx={{
                                    position: "absolute",
                                    bottom: -4,
                                    left: 0,
                                    width: 40,
                                    height: 3,
                                    bgcolor: "grey.300",
                                    borderRadius: 2,
                                }}
                            />
                        </Box>
                        <Box sx={{ display: "flex", gap: 2 }}>
                            <Skeleton variant="rectangular" width={100} height={40} />
                            <Skeleton variant="rectangular" width={120} height={40} />
                        </Box>
                    </Box>

                    {/* Two-column body */}
                    <Grid container spacing={3}>
                        {/* Left form column */}
                        <Grid size={{ xs: 12, md: 6 }}>
                            {/* Mimic stacked inputs */}
                            {Array.from({ length: 5 }).map((_, i) => (
                                <Skeleton key={i} height={56} sx={{ mb: 3 }} />
                            ))}
                            {/* Multiline description */}
                            <Skeleton variant="rectangular" height={120} sx={{ mb: 3, borderRadius: 1 }} />
                            {/* Dropzone */}
                            <Skeleton variant="rectangular" height={120} sx={{ borderRadius: 1 }} />
                        </Grid>

                        {/* Right image column */}
                        <Grid size={{ xs: 12, md: 6 }} display={"flex"} justifyContent="center" alignItems="center">
                            <Box sx={{ width: "100%" }}>
                                <Skeleton variant="rectangular" height={650} sx={{ borderRadius: 2 }} />
                            </Box>
                        </Grid>
                    </Grid>
                </Box>
            </Box>
        );
    }

    // --- Form ready ---
    const previewSrc = watchFile?.preview || props.product?.pictureUrl;

    return (
        <Box sx={{ pt: 4 }}>
            <Box component={Paper} sx={{ p: 4, maxWidth: "lg", mx: "auto" }}>
                {/* Accent heading */}
                <Box
                    sx={{
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center",
                        mb: 3,
                    }}
                >
                    <Box sx={{ display: "inline-block", position: "relative", mb: 2 }}>
                        <Typography variant="h4" sx={{ fontWeight: 800 }}>
                            Product Details
                        </Typography>
                        <Box
                            sx={{
                                position: "absolute",
                                bottom: -4,
                                left: 0,
                                width: 40,
                                height: 3,
                                bgcolor: "primary.main",
                                borderRadius: 2,
                            }}
                        />
                    </Box>

                    <Box>
                        <Button onClick={props.onFormCancel} variant="contained" color="inherit" sx={{ mr: 2 }}>
                            Cancel
                        </Button>
                        <Button form="submit-button" variant="contained" color="success" type="submit" disabled={isSubmitting || !isValid}>
                            {isSubmitting ? "Saving..." : "Submit"}
                        </Button>
                    </Box>
                </Box>

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

                        <Grid size={{ xs: 12, md: 6 }} display={"flex"} justifyContent="center" alignItems="center" >
                            {previewSrc ? <Box component="img" src={previewSrc} alt="product preview" sx={{ borderRadius: 5, maxHeight: 650, maxWidth: 500, objectFit: "cover" }} /> : <NoImagePlaceholder />}
                        </Grid>
                    </Grid>
                </form>
            </Box>
        </Box>
    );
}
