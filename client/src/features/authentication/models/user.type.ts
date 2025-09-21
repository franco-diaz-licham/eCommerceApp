export type UserResponse = {
    id: string;
    userName: string;
    email: string;
    AddressId?: number;
    Address?: AddressResponse;
    roles: string[];
};

export type AddressResponse = {
    id: number;
    line1: string;
    line2?: string | null;
    city: string;
    state: string;
    postalCode: string;
    country: string;
};

export interface AddressCreateDto {
    line1: string;
    line2?: string | null;
    city: string;
    state: string;
    postalCode: string;
    country: string;
}

export interface AddressUpdateDto extends AddressCreateDto {
    id: number;
}
