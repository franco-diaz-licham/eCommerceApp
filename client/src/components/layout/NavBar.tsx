import { AppBar, Badge, Box, Container, IconButton, LinearProgress, Stack, Toolbar, Typography, Link as MuiLink } from "@mui/material";
import { DarkMode, LightMode, ShoppingCart } from "@mui/icons-material";
import { Link, NavLink } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../app/store/store";
import { setDarkMode } from "./uiSlice";
import { useBasket } from "../../hooks/useBasket";
import UserMenu from "./UserMenu";
import { useUserInfoQuery } from "../../features/authentication/api/account.api";
import Whitelogo from "../../assets/white-logo.png";
import { useState } from "react";
import MenuIcon from "@mui/icons-material/Menu";
import NavBarDraw from "./NavBarDraw";

export interface NavBarLink {
    title: string;
    path: string;
}

const LINKS = [
    { title: "Home", path: "/" },
    { title: "Products", path: "/products" },
    { title: "Contact", path: "/contact" },
];

const AUTH_LINKS = [
    { title: "Login", path: "/login" },
    { title: "Register", path: "/register" },
];

const NAV_STYLES = {
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
    const [drawerOpen, setDrawerOpen] = useState(false);
    const handleDrawerToggle = () => setDrawerOpen((prevState) => !prevState);

    return (
        <>
            <AppBar position="sticky" color="primary" elevation={0} sx={{ height: 64 }}>
                <Container maxWidth="xl">
                    <Toolbar sx={{ gap: 2 }} disableGutters>
                        <IconButton color="default" aria-label="open drawer" edge="start" onClick={handleDrawerToggle} sx={{ ml: 1, display: { sm: "none" } }}>
                            <MenuIcon fontSize="large" sx={{ color: "white" }} />
                        </IconButton>
                        <Typography component={NavLink} to="/">
                            <Box
                                component="img"
                                sx={{
                                    height: 50,
                                    width: 160,
                                    display: { xs: "none", sm: "block" },
                                }}
                                alt="ego store logo."
                                src={Whitelogo}
                            />
                        </Typography>
                        <Stack direction="row" spacing={2} sx={{ ml: 3, display: { xs: "none", sm: "initial" } }}>
                            {LINKS.map(({ title, path }) => (
                                <MuiLink component={NavLink} to={path} key={path} sx={NAV_STYLES}>
                                    {title}
                                </MuiLink>
                            ))}
                            {!user &&
                                AUTH_LINKS.map(({ title, path }) => (
                                    <MuiLink component={NavLink} to={path} key={path} sx={NAV_STYLES}>
                                        {title}
                                    </MuiLink>
                                ))}
                        </Stack>

                        <Box sx={{ flexGrow: 1 }} />

                        <IconButton onClick={() => dispatch(setDarkMode())} size="large" sx={{ color: "inherit" }}>
                            {darkMode ? <LightMode sx={{ color: "white" }} /> : <DarkMode sx={{ color: "white" }} />}
                        </IconButton>

                        <IconButton component={Link} to="/basket" size="large" sx={{ color: "inherit" }}>
                            <Badge badgeContent={itemCount} color="secondary">
                                <ShoppingCart />
                            </Badge>
                        </IconButton>
                        {user && <UserMenu user={user} />}
                    </Toolbar>
                </Container>

                {isLoading && (
                    <Box sx={{ width: "100%" }} position={"fixed"} marginTop={8}>
                        <LinearProgress color="secondary" sx={{ height: 10 }} />
                    </Box>
                )}
            </AppBar>
            {/* Drawer */}
            <NavBarDraw drawerOpen={drawerOpen} closeDraw={handleDrawerToggle} links={LINKS} authLinks={AUTH_LINKS} user={user} />
        </>
    );
}
