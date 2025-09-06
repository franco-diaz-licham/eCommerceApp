import { type FieldValues, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Box, Button, Grid, Paper, Typography } from "@mui/material";
import { useEffect } from "react";
import type { ProductFormData } from "../../product/types/product.types";
import { createProductSchema, type CreateProductSchema } from "../../product/types/createProductSchema";
import { useFetchFiltersQuery } from "../../product/services/product.api";
import { handleApiError } from "../../../lib/utils";
import AppTextInput from "../../../components/ui/AppSelectTextInput";
import AppSelectInput from "../../../components/ui/AppSelectInput";
import AppDropzone from "../../../components/ui/AppDropzone";
import { useCreateProductMutation, useUpdateProductMutation } from "../services/adminApi";
// import { LoadingButton } from "@mui/lab"; // if you want loading submit

type Props = {
    setEditMode: (value: boolean) => void;
    product: ProductFormData | null;
    refetch: () => void;
    setSelectedProduct: (value: ProductFormData | null) => void;
};

type FileWithPreview = File & { preview?: string };

export default function ProductForm({ setEditMode, product, refetch, setSelectedProduct }: Props) {
    const {
        control,
        handleSubmit,
        watch,
        reset,
        setError,
        formState: { isSubmitting },
    } = useForm<CreateProductSchema>({
        mode: "onTouched",
        resolver: zodResolver(createProductSchema),
        defaultValues: {
            name: "",
            description: "",
            price: 100,
            type: "",
            brand: "",
            quantityInStock: 1,
            pictureUrl: "",
            file: undefined,
        },
    });

    // The file stored by RHF should be a File with an attached preview set by AppDropzone
    const watchFile = watch("file") as FileWithPreview | undefined;

    const { data } = useFetchFiltersQuery();
    const [createProduct] = useCreateProductMutation();
    const [updateProduct] = useUpdateProductMutation();

    // On edit, reset form with product values but ensure file is cleared
    useEffect(() => {
        if (product) reset({ ...product, file: undefined });
    }, [product, reset]);

    // Revoke the CURRENT preview URL when it changes/unmounts
    useEffect(() => {
        const url = watchFile?.preview;
        return () => {
            if (url) URL.revokeObjectURL(url);
        };
    }, [watchFile?.preview]);

    // FormData helper: append Blob as-is, stringify others
    const createFormData = (items: FieldValues) => {
        const formData = new FormData();
        Object.entries(items as Record<string, unknown>).forEach(([key, value]) => {
            if (value == null) return;
            if (value instanceof Blob) formData.append(key, value);
            else if (Array.isArray(value)) value.forEach((v) => formData.append(key, String(v)));
            else if (typeof value === "object") formData.append(key, JSON.stringify(value));
            else formData.append(key, String(value));
        });
        return formData;
    };

    const onSubmit = async (data: CreateProductSchema) => {
        try {
            const formData = createFormData(data);
            if (watchFile) formData.append("file", watchFile as File);
            if (product?.id) await updateProduct({ id: product.id, data: formData }).unwrap();
            else await createProduct(formData).unwrap();

            setEditMode(false);
            setSelectedProduct(null);
            refetch();
        } catch (error) {
            console.log(error);
            handleApiError<CreateProductSchema>(error, setError, ["brand", "description", "file", "name", "pictureUrl", "price", "quantityInStock", "type"]);
        }
    };

    return (
        <Box component={Paper} sx={{ p: 4, maxWidth: "lg", mx: "auto" }}>
            <Typography variant="h4" sx={{ mb: 4 }}>
                Product details
            </Typography>

            <form onSubmit={handleSubmit(onSubmit)} noValidate>
                <Grid container spacing={3}>
                    <Grid /* item xs={12} */ size={12}>
                        <AppTextInput control={control} name="name" label="Product name" />
                    </Grid>

                    <Grid /* item xs={12} md={6} */ size={6}>{data?.brands && <AppSelectInput items={data.brands} control={control} name="brand" label="Brand" />}</Grid>

                    <Grid /* item xs={12} md={6} */ size={6}>
                        {data?.types && ( // ✅ guard types correctly
                            <AppSelectInput items={data.types} control={control} name="type" label="Type" />
                        )}
                    </Grid>

                    <Grid /* item xs={12} md={6} */ size={6}>
                        <AppTextInput type="number" control={control} name="price" label="Price in cents" />
                    </Grid>

                    <Grid /* item xs={12} md={6} */ size={6}>
                        <AppTextInput type="number" control={control} name="quantityInStock" label="Quantity in stock" />
                    </Grid>

                    <Grid /* item xs={12} */ size={12}>
                        <AppTextInput control={control} multiline rows={4} name="description" label="Description" />
                    </Grid>

                    <Grid /* item xs={12} */ size={12} display="flex" justifyContent="space-between" alignItems="center">
                        <AppDropzone name="file" control={control} />
                        {watchFile?.preview ? <img src={watchFile.preview} alt="preview of image" style={{ maxHeight: 200 }} /> : product?.pictureUrl ? <img src={product.pictureUrl} alt="existing product" style={{ maxHeight: 200 }} /> : null}
                    </Grid>
                </Grid>

                <Box display="flex" justifyContent="space-between" sx={{ mt: 3 }}>
                    <Button onClick={() => setEditMode(false)} variant="contained" color="inherit">
                        Cancel
                    </Button>

                    <Button variant="contained" color="success" type="submit" disabled={isSubmitting}>
                        {isSubmitting ? "Saving..." : "Submit"}
                    </Button>
                    {/*
          <LoadingButton loading={isSubmitting} variant="contained" color="success" type="submit">
            Submit
          </LoadingButton>
          */}
                </Box>
            </form>
        </Box>
    );
}
