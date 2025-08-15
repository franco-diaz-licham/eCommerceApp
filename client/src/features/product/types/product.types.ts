import type { PhotoResponse } from "../../../lib/photo/photo.types";
import type { BaseQueryParams } from "../../../types/baseQueryParams.type";
import type { BrandResponse } from "../../../entities/brand/brand.types";
import type { ProductTypeResponse } from "../../../entities/productType/productTypeResponse.type";

/** Product response DTO. */
export interface ProductResponse {
    id: number;
    name: string;
    description: string;
    price: number;
    productTypeId: number;
    productType: ProductTypeResponse;
    brandId: number;
    brand: BrandResponse;
    photoId: number;
    photo: PhotoResponse;
    quantityInStock: number;
}

/** DTO used for creating a product. */
export interface ProductCreate {
    name: string;
    description: string;
    price: number;
    productTypeId: number;
    brandId: number;
    photoId: number;
    quantityInStock: number;
}

/** DTO used for updating a product. */
export interface ProductUpdate extends ProductCreate {
    id: number;
}

/** DTO for the form to create a product. */
export interface ProductFormData {
    id?: number;
    name: string;
    description: string;
    price: number;
    productTypeId: number;
    brandId: number;
    photoId: number;
    quantityInStock: number;
}

/** Product query params. */
export interface ProductQueryParams extends BaseQueryParams {
    types?: number[];
    brands?: number[];
}
