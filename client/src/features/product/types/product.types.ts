import type { PhotoResponse } from "../../../lib/photo/photo.types";
import type { BaseQueryParams } from "../../../types/baseQueryParams.type";
import type { BrandResponse } from "../../../entities/brand/brand.types";
import type { ProductTypeResponse } from "../../../entities/productType/productTypeResponse.type";

/** Product response DTO. */
export interface ProductResponse {
    id: number;
    name: string;
    description: string;
    unitPrice: number;
    productTypeId: number;
    productType?: ProductTypeResponse;
    brandId: number;
    brand?: BrandResponse;
    photoId: number;
    photo?: PhotoResponse;
    quantityInStock: number;
}

/** DTO used for creating a product. */
export interface ProductCreate {
    name: string;
    description: string;
    unitPrice: number;
    productTypeId: number;
    brandId: number;
    quantityInStock: number;
    photo?: File;
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
    unitPrice: number;
    productTypeId: number;
    brandId: number;
    /** Preview of picture */
    pictureUrl?: string;
    photo?: File;
    quantityInStock: number;
}

/** Product query params. */
export interface ProductQueryParams extends BaseQueryParams {
    productTypeIds?: number[];
    brandIds?: number[];
}

/** Specific filters for product model. */
export interface ProductFilters {
    productTypes: ProductTypeResponse[];
    brands: BrandResponse[];
}

/** Filterpreview type to see photo previews. */
export type FileWithPreview = File & { preview?: string };