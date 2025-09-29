import { Grid } from "@mui/material";
import type { ProfileFormData } from "../models/user.type";
import TextInput from "../../../components/ui/TextInput";
import { profileSchema, type ProfileSchema } from "../models/profileSchema";
import { useEffect, useRef } from "react";
import { useForm, useFormState } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

/** Functional props. */
type ProfileFormProps = {
    model: ProfileFormData | null;
    onFormSubmit: (data: ProfileFormData) => void;
    onDisabledChanged: (disabled: boolean) => void;
};

export default function ProfileForm(props: ProfileFormProps) {
    const methods = useForm<ProfileSchema>({
        mode: "onChange",
        resolver: zodResolver(profileSchema),
        defaultValues: {
            line1: "",
            line2: "",
            city: "",
            state: "",
            postalCode: "",
            country: "",
        },
    });
    const { control, handleSubmit, reset } = methods;
    const { isSubmitting, isValid, isDirty } = useFormState({ control });
    const initialized = useRef(false);

    useEffect(() => {
        props.onDisabledChanged(isSubmitting || !isValid || !isDirty);
    }, [isSubmitting, isValid, isDirty, props]);

    // Set internal model with passed parent model.
    useEffect(() => {
        if (initialized.current) return;
        const profile = props.model;
        if (!profile) return;

        reset(
            {
                line1: profile.line1,
                line2: profile.line2,
                city: profile.city,
                state: profile.state,
                postalCode: profile.postalCode,
                country: profile.country,
            },
            { keepDirty: false }
        );

        initialized.current = true;
    }, [props.model, reset]);

    /** Handle submit and set internal model with edit. */
    const handleOnSubmit = (form: ProfileSchema) => {
        props.onFormSubmit({ ...form });
        reset({ ...form }, { keepDirty: false });
    };

    return (
        <form onSubmit={handleSubmit(handleOnSubmit)} noValidate id="profileForm">
            <TextInput type="text" control={control} name="line1" label="Line 1" />
            <TextInput sx={{ mt: 3 }} type="text" control={control} name="line2" label="Line 2" />
            <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 6 }}>
                    <TextInput sx={{ mt: 3 }} type="text" control={control} name="city" label="City" />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                    <TextInput sx={{ mt: 3 }} type="text" control={control} name="state" label="State" />
                </Grid>
            </Grid>
            <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 6 }}>
                    <TextInput sx={{ mt: 3 }} type="text" control={control} name="postalCode" label="Postal Code" />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                    <TextInput sx={{ mt: 3 }} type="text" control={control} name="country" label="Country" />
                </Grid>
            </Grid>
        </form>
    );
}
