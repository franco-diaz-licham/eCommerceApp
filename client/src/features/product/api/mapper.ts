import type { CreateProductSchema } from "../components/productForm/createProductSchema";
import type { ProductCreate, ProductFormData, ProductResponse, ProductUpdate } from "../models/product.types";

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

/** Maps prodcut form data DTO to product create DTO. */
export function mapToProductCreate(data: ProductFormData): ProductCreate {
    return {
        name: data.name,
        description: data.description,
        unitPrice: data.unitPrice,
        productTypeId: data.productTypeId,
        brandId: data.brandId,
        photo: data.photo,
        quantityInStock: data.quantityInStock,
    };
}

/** Maps prodcut form data DTO to product create DTO. */
export function mapToProductUpdate(data: ProductFormData): ProductUpdate {
    return {
        id: data.id!,
        name: data.name,
        description: data.description,
        unitPrice: data.unitPrice,
        productTypeId: data.productTypeId,
        brandId: data.brandId,
        photo: data.photo,
        quantityInStock: data.quantityInStock,
    };
}

/** Map Zod output to form DTO. */
export function toProductFormData(input: CreateProductSchema, id?: number): ProductFormData {
    return {
        id,
        name: input.name,
        description: input.description,
        unitPrice: input.price,
        productTypeId: input.type,
        brandId: input.brand,
        pictureUrl: input.pictureUrl,
        quantityInStock: input.quantityInStock,
        photo: input.photo,
    };
}
