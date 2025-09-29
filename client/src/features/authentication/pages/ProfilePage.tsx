import { Box, Button, Paper, Typography } from "@mui/material";
import { toast } from "react-toastify";
import { useCreateUserAddressMutation, useFetchAddressQuery, useUpdateUserAddressMutation } from "../api/account.api";
import Header from "../../../components/ui/Header";
import SaveIcon from "@mui/icons-material/Save";
import { useState } from "react";
import ProfileForm from "../components/ProfileForm";
import { mapToAddressUpdateDto, mapToProfileFormData } from "../api/mappers";
import type { ProfileFormData } from "../models/user.type";
import { mapToAddressCreateDto } from "../../checkout/api/checkoutMapper";
import { getErrorMessage } from "../../../lib/utils";

export default function ProfilePage() {
    const { data: address } = useFetchAddressQuery();
    const [createAddress] = useCreateUserAddressMutation();
    const [updateAddress] = useUpdateUserAddressMutation();
    const [formValid, setFormValid] = useState(false);

    const handleOnSubmit = async (dto: ProfileFormData) => {
        try {
            if (address && address.id) await updateAddress(mapToAddressUpdateDto(dto, address.id)).unwrap();
            else createAddress(mapToAddressCreateDto(dto)).unwrap();
            toast.success("Profile saved successfully.");
        } catch (e: unknown) {
            toast.error(getErrorMessage(e, "Failed to save profile."));
        }
    };

    return (
        <Box sx={{ pt: 4 }}>
            {/* Header */}
            <Header title="My Profile">
                <Box>
                    <Button form="profileForm" variant="contained" color="success" type="submit" disabled={formValid} sx={{ textTransform: "none" }} size="small">
                        Save <SaveIcon sx={{ ml: 1 }} />
                    </Button>
                </Box>
            </Header>

            {/* Content */}
             <Box component={Paper} sx={{ p: 4 }}>
                <Typography variant="h6" fontWeight="bold" marginBottom={3}>
                    Address Information
                </Typography>
                <ProfileForm model={address ? mapToProfileFormData(address) : null} onFormSubmit={handleOnSubmit} onDisabledChanged={setFormValid} />
            </Box>
        </Box>
    );
}
