import type { AddressCreateDto } from "../../authentication/models/user.type";
import type { ShippingAddressDto } from "../../order/models/order.type";
import type { ShippingAddressModel } from "../models/ui";

/** Maps product reponse to a product form data DTO. */
export function mapToAddressCreateDto(data: ShippingAddressModel): AddressCreateDto {
    return {
        line1: data.line1,
        line2: data.line2,
        postalCode: data.postalCode,
        city: data.city,
        state: data.state ?? "",
        country: data.country,
    };
}

/** Maps shipping address model to the equivalent Dto. */
export function mapToShippingAddressDto(data: ShippingAddressModel): ShippingAddressDto {
    return {
        recipientName: data.name,
        line1: data.line1,
        line2: data.line2,
        postalCode: data.postalCode,
        city: data.city,
        state: data.state ?? "",
        country: data.country,
    };
}
