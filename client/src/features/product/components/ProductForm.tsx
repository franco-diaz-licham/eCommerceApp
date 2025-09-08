import { useForm, useFormState, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Box, Button, Grid, Paper, Typography } from "@mui/material";
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
    const { isSubmitting, isValid, isDirty, submitCount, errors } = useFormState({ control });
    const values = useWatch({ control });
    const initialized = useRef(false);
    /** The file stored by RHF should be a File with an attached preview set by Dropzone component. */
    const watchFile = watch("photo") as FileWithPreview | undefined;
    const { data } = useFetchFiltersQuery();

    // On edit, reset form with product values but ensure file is cleared
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

    // Revoke the CURRENT preview URL when it changes/unmounts
    useEffect(() => {
        const url = watchFile?.preview;
        return () => {
            if (url) URL.revokeObjectURL(url);
        };
    }, [watchFile?.preview]);

    /** Handles form submission and callbacks to the parent component. */
    const handleOnSubmit = (data: CreateProductSchema) => {
        props.onFormSubmit(toProductFormData(data, props.product?.id));
    };

    return (
        <Box component={Paper} sx={{ p: 4, maxWidth: "lg", mx: "auto" }}>
            <Typography variant="h4" sx={{ mb: 4 }}>
                Product details
            </Typography>
            <form onSubmit={handleSubmit(handleOnSubmit)} noValidate>
                <Grid container spacing={3}>
                    <Grid size={12}>
                        <TextInput control={control} name="name" label="Product name" />
                    </Grid>
                    <Grid size={6}>{data?.brands && <SelectInput items={data.brands.map((x) => ({ value: x.id, label: x.name }))} control={control} name="brand" label="Brand" />}</Grid>
                    <Grid size={6}>{data?.productTypes && <SelectInput items={data.productTypes.map((x) => ({ value: x.id, label: x.name }))} control={control} name="type" label="Type" />}</Grid>
                    <Grid size={6}>
                        <TextInput type="number" control={control} name="price" label="Price in cents" />
                    </Grid>
                    <Grid size={6}>
                        <TextInput type="number" control={control} name="quantityInStock" label="Quantity in stock" />
                    </Grid>
                    <Grid size={12}>
                        <TextInput control={control} multiline rows={4} name="description" label="Description" />
                    </Grid>
                    <Grid size={12} display="flex" justifyContent="space-between" alignItems="center">
                        <Dropzone name="photo" control={control} />
                        {watchFile?.preview ? <img src={watchFile.preview} alt="preview of image" style={{ maxHeight: 200 }} /> : props.product?.pictureUrl ? <img src={props.product.pictureUrl} alt="existing product" style={{ maxHeight: 200 }} /> : null}
                    </Grid>
                </Grid>

                <Box display="flex" justifyContent="space-between" sx={{ mt: 3 }}>
                    <Button onClick={() => props.onFormCancel()} variant="contained" color="inherit">
                        Cancel
                    </Button>
                    <Button variant="contained" color="success" type="submit" disabled={isSubmitting || !isValid}>
                        {isSubmitting ? "Saving..." : "Submit"}
                    </Button>
                </Box>
                {/* Debugging */}
                {/* <div>
                    <pre>errors: {JSON.stringify(errors, null, 2)}</pre>
                    <pre>isValid: {String(isValid)}</pre>
                    <pre>
                        isDirty: {String(isDirty)} submitCount: {submitCount}
                    </pre>
                    <pre>values: {JSON.stringify(values, null, 2)}</pre>
                </div> */}
            </form>
        </Box>
    );
}
