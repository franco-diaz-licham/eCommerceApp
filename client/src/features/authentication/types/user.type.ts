export type UserResponse = {
    id: string;
    userName: string;
    email: string;
    AddressId: number;
    Address: Address;
    roles: string[];
};

export type Address = {
    name: string;
    line1: string;
    line2?: string | null;
    city: string;
    state: string;
    postal_code: string;
    country: string;
};
