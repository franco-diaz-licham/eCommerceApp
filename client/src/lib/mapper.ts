import type { ProductFormData, ProductResponse } from "../features/product/types/product.types";

/** Maps product reponse to a product form data DTO. */
export function mapToProductFormData(data: ProductResponse) {
    return {
        id: data.id,
        name: data.name,
        description: data.description,
        unitPrice: data.unitPrice,
        productTypeId: data.productTypeId,
        brandId: data.brandId,
        photoId: data.photoId,
        pictureUrl: data.photo?.publicUrl,
        quantityInStock: data.quantityInStock,
    };
}

/** MAprs product form data to Product response. */
export function mapToProductResponse(data: ProductFormData): ProductResponse {
    return {
        id: data.id!,
        name: data.name,
        description: data.description,
        unitPrice: data.unitPrice,
        productTypeId: data.productTypeId,
        brandId: data.brandId,
        photoId: data.photoId,
        quantityInStock: data.quantityInStock,
    };
}
