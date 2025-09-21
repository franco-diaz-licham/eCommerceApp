import { Box, Button } from "@mui/material";
import { useState } from "react";
import { useAppDispatch, useAppSelector } from "../../../app/store/store";
import { useCreateProductMutation, useDeleteProductMutation, useFetchProductsQuery, useUpdateProductMutation } from "../api/product.api";
import ProductForm from "../components/productForm/ProductForm";
import type { ProductFormData, ProductResponse } from "../models/product.types";
import { mapToProductCreate, mapToProductFormData, mapToProductUpdate } from "../api/mapper";
import { setPageNumber, setSearchTerm } from "../api/productSlice";
import AddCircleIcon from "@mui/icons-material/AddCircle";
import SearchField from "../components/SearchField";
import Header from "../../../components/ui/Header";
import InventoryTable from "../components/InventoryTable";
import SaveIcon from "@mui/icons-material/Save";
import CancelIcon from "@mui/icons-material/Cancel";

export default function InventoryPage() {
    const params = useAppSelector((state) => state.products);
    const { data, refetch } = useFetchProductsQuery(params);
    const dispatch = useAppDispatch();
    const [editMode, setEditMode] = useState(false);
    const [selectedProduct, setSelectedProduct] = useState<ProductResponse | null>(null);
    const [deleteProduct] = useDeleteProductMutation();
    const [createProduct] = useCreateProductMutation();
    const [updateProduct] = useUpdateProductMutation();
    const [formValid, setFormValid] = useState(false);

    const handleSelectProduct = (product: ProductResponse) => {
        setSelectedProduct(product);
        setEditMode(true);
    };

    const handleDeleteProduct = async (id: number) => {
        try {
            await deleteProduct(id);
            refetch();
        } catch (error) {
            console.log(error);
        }
    };

    const handleOnSubmit = async (data: ProductFormData) => {
        try {
            if (data?.id) {
                const product = mapToProductUpdate(data);
                await updateProduct(product).unwrap();
            } else {
                const product = mapToProductCreate(data);
                await createProduct(product).unwrap();
            }

            setEditMode(false);
            setSelectedProduct(null);
            refetch();
        } catch (error) {
            console.log(error);
        }
    };

    const handleCancel = () => {
        setSelectedProduct(null);
        setEditMode(false);
    };

    if (editMode) {
        return (
            <Box sx={{ pt: 4 }}>
                <Header title="Product Details">
                    <Box>
                        <Button onClick={handleCancel} variant="contained" color="inherit" sx={{ mr: 1, textTransform: "none" }} size="small">
                            Cancel <CancelIcon sx={{ ml: 1 }} />
                        </Button>
                        <Button form="submit-button" variant="contained" color="success" type="submit" disabled={formValid} sx={{ textTransform: "none" }} size="small">
                            Save <SaveIcon sx={{ ml: 1 }} />
                        </Button>
                    </Box>
                </Header>
                <ProductForm onFormCancel={handleCancel} product={selectedProduct ? mapToProductFormData(selectedProduct) : null} onFormSubmit={handleOnSubmit} onDisabledChanged={setFormValid} />
            </Box>
        );
    }

    return (
        <Box sx={{ pt: 4 }}>
            {/* Header */}
            <Header title="Inventory">
                <SearchField value={params.searchTerm ?? ""} onSearchChange={(value) => dispatch(setSearchTerm(value))} />
                <Button onClick={() => setEditMode(true)} variant="contained" sx={{ textTransform: "none", px: 3, ml: 2 }} color="primary" size="small">
                    Create <AddCircleIcon sx={{ ml: 1 }} />
                </Button>
            </Header>

            {/* Table */}
            <InventoryTable data={data} onPaginationChanged={(page: number) => dispatch(setPageNumber(page))} onProductDeleted={handleDeleteProduct} onProductSelected={handleSelectProduct} />
        </Box>
    );
}
