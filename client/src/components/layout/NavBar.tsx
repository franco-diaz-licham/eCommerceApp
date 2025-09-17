import { AppBar, Badge, Box, Container, IconButton, LinearProgress, Stack, Toolbar, Typography, Link as MuiLink } from "@mui/material";
import { DarkMode, LightMode, ShoppingCart } from "@mui/icons-material";
import { Link, NavLink } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../app/store/store";
import { setDarkMode } from "./uiSlice";
import { useBasket } from "../../hooks/useBasket";
import UserMenu from "./UserMenu";
import { useUserInfoQuery } from "../../features/authentication/services/account.api";
import Whitelogo from "../../assets/white-logo.png";

const leftLinks = [
    { title: "Home", path: "/" },
    { title: "Products", path: "/products" },
    { title: "Contact", path: "/contact" },
];

const rightLinks = [
    { title: "Login", path: "/login" },
    { title: "Register", path: "/register" },
];

const navStyles = {
    color: "inherit",
    typography: "",
    textDecoration: "none",
    "&:hover": {
        color: "grey.500",
    },
    "&.active": {
        color: "#baecf9",
    },
};

export default function NavBar() {
    const { data: user } = useUserInfoQuery();
    const { itemCount } = useBasket();
    const { isLoading, darkMode } = useAppSelector((state) => state.ui);
    const dispatch = useAppDispatch();

    return (
        <AppBar position="sticky" color="primary" elevation={0}>
            <Container maxWidth="xl">
                <Toolbar sx={{ gap: 2 }} disableGutters>
                    <Typography component={NavLink} to="/">
                        <Box
                            component="img"
                            sx={{
                                height: 50,
                                width: 160,
                            }}
                            alt="ego store logo."
                            src={Whitelogo}
                        />
                    </Typography>
                    <Stack direction="row" spacing={2} sx={{ ml: 3, display: { xs: "none", md: "flex" } }}>
                        {leftLinks.map(({ title, path }) => (
                            <MuiLink component={NavLink} to={path} key={path} sx={navStyles}>
                                {title}
                            </MuiLink>
                        ))}
                    </Stack>

                    <Box sx={{ flexGrow: 1 }} />
                    <IconButton onClick={() => dispatch(setDarkMode())}>{darkMode ? <LightMode sx={{ color: "white" }} /> : <DarkMode sx={{ color: "white" }} />}</IconButton>
                    <IconButton component={Link} to="/basket" size="large" sx={{ color: "inherit" }}>
                        <Badge badgeContent={itemCount} color="secondary">
                            <ShoppingCart />
                        </Badge>
                    </IconButton>
                    {user ? (
                        <UserMenu user={user} />
                    ) : (
                        rightLinks.map(({ title, path }) => (
                            <MuiLink component={NavLink} to={path} key={path} sx={navStyles}>
                                {title}
                            </MuiLink>
                        ))
                    )}
                </Toolbar>
            </Container>

            {isLoading && (
                <Box sx={{ width: "100%" }} position={"fixed"} marginTop={8}>
                    <LinearProgress color="secondary" sx={{height: 10}} />
                </Box>
            )}
        </AppBar>
    );
}
