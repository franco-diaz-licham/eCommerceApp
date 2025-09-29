import type { AddressResponse, ProfileFormData } from "../models/user.type";

export function mapToProfileFormData(data: AddressResponse) {
    return {
        id: !data.id ? undefined : data.id,
        line1: data.line1,
        line2: data.line2,
        city: data.city,
        state: data.state,
        postalCode: data.postalCode,
        country: data.country,
    };
}

export function mapToAddressCreateDto(data: ProfileFormData) {
    return {
        line1: data.line1,
        line2: data.line2,
        city: data.city,
        state: data.state,
        postalCode: data.postalCode,
        country: data.country,
    };
}


export function mapToAddressUpdateDto(data: ProfileFormData, id: number) {
    return {
        id,
        line1: data.line1,
        line2: data.line2,
        city: data.city,
        state: data.state,
        postalCode: data.postalCode,
        country: data.country,
    };
}
