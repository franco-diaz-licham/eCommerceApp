import { Box, Button, Container, Paper, TextField, Typography } from "@mui/material";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link } from "react-router-dom";
import { useRegisterMutation } from "../services/account.api";
import { registerSchema, type RegisterSchema } from "../types/registerSchema";
import { useAppSelector } from "../../../app/store/store";
import DarkLogo from "../../../assets/dark-logo.png";
import WhiteLogo from "../../../assets/white-logo.png";

export default function RegisterFormPage() {
    const { darkMode } = useAppSelector((state) => state.ui);
    const [registerUser] = useRegisterMutation();

    const {
        register,
        handleSubmit,
        setError,
        formState: { errors, isValid, isLoading },
    } = useForm<RegisterSchema>({
        mode: "onTouched",
        resolver: zodResolver(registerSchema),
    });

    // keep your original logic
    const onSubmit = async (data: RegisterSchema) => {
        try {
            await registerUser(data).unwrap();
        } catch (error) {
            const apiError = error as { message: string };
            if (apiError.message && typeof apiError.message === "string") {
                const errorArray = apiError.message.split(",");
                errorArray.forEach((e) => {
                    if (e.includes("Password")) {
                        setError("password", { message: e });
                    } else if (e.includes("Email")) {
                        setError("email", { message: e });
                    }
                });
            }
        }
    };

    return (
        <Box
            sx={{
                minHeight: "calc(100vh - 230px)",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                px: 2,
            }}
        >
            <Container component={Paper} maxWidth="sm" sx={{ borderRadius: 3, p: 4 }}>
                <Box display="flex" flexDirection="column" alignItems="center" gap={2}>
                    <Box component="img" src={darkMode ? WhiteLogo : DarkLogo} alt="Logo" sx={{ height: 80, width: 300 }} />
                    <Typography variant="h5" sx={{ fontWeight: 600 }}>
                        Sign up
                    </Typography>

                    <Box component="form" onSubmit={handleSubmit(onSubmit)} width="100%" display="flex" flexDirection="column" gap={3} mt={2}>
                        <TextField fullWidth label="Email" {...register("email")} error={!!errors.email} helperText={errors.email?.message} autoComplete="email" />
                        <TextField fullWidth label="Password" type="password" {...register("password")} error={!!errors.password} helperText={errors.password?.message} autoComplete="new-password" />

                        <Button disabled={isLoading || !isValid} variant="contained" type="submit" size="large">
                            Sign up
                        </Button>

                        <Typography sx={{ textAlign: "center" }}>
                            Already have an account?
                            <Typography component={Link} to="/login" color="primary" sx={{ ml: 1, fontWeight: 500 }}>
                                Sign in
                            </Typography>
                        </Typography>
                    </Box>
                </Box>
            </Container>
        </Box>
    );
}
