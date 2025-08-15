
/** Photo API response model. */
export interface PhotoResponse {
    id: string;
    fileName: string;
    publicUrl: string;
    publicId: string;
}

/** Photo create DTO. */
export interface PhotoCreate {
    image: File[];
}

/** Photo update DTO. */
export interface PhotoUpdate extends PhotoCreate {
    id: number;
}

