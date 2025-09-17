import { Box, Button, Container, Paper, TextField, Typography } from "@mui/material";
import { useForm } from "react-hook-form";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { zodResolver } from "@hookform/resolvers/zod";
import { useLazyUserInfoQuery, useLoginMutation } from "../services/account.api";
import { loginSchema, type LoginSchema } from "../types/loginSchema";
import DarkLogo from "../../../assets/dark-logo.png";
import WhiteLogo from "../../../assets/white-logo.png";
import { useAppSelector } from "../../../app/store/store";

export default function LoginFormPage() {
    const { darkMode } = useAppSelector((state) => state.ui);
    const [login, { isLoading }] = useLoginMutation();
    const [fetchUserInfo] = useLazyUserInfoQuery();
    const location = useLocation();
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<LoginSchema>({
        mode: "onTouched",
        resolver: zodResolver(loginSchema),
    });
    const navigate = useNavigate();

    const onSubmit = async (data: LoginSchema) => {
        await login(data);
        await fetchUserInfo();
        navigate(location.state?.from || "/products");
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
                    <Box
                        component="img"
                        src={darkMode ? WhiteLogo : DarkLogo}
                        alt="Logo"
                        sx={{
                            height: 80,
                            width: 300,
                        }}
                    />
                    <Typography variant="h5" sx={{ fontWeight: 600 }}>
                        Sign in
                    </Typography>

                    <Box component="form" onSubmit={handleSubmit(onSubmit)} width="100%" display="flex" flexDirection="column" gap={3} mt={2}>
                        <TextField fullWidth label="Email" {...register("email")} error={!!errors.email} helperText={errors.email?.message} />
                        <TextField fullWidth label="Password" type="password" {...register("password")} error={!!errors.password} helperText={errors.password?.message} />
                        <Button disabled={isLoading} variant="contained" type="submit" size="large">
                            Sign in
                        </Button>
                        <Typography sx={{ textAlign: "center" }}>
                            Don&apos;t have an account?
                            <Typography component={Link} to="/register" color="primary" sx={{ ml: 1, fontWeight: 500 }}>
                                Sign up
                            </Typography>
                        </Typography>
                    </Box>
                </Box>
            </Container>
        </Box>
    );
}
