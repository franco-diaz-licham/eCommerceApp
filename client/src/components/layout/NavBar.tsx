import { AppBar, Badge, Box, Container, IconButton, LinearProgress, List, ListItem, Toolbar, Typography } from "@mui/material";
import { DarkMode, LightMode, ShoppingCart } from "@mui/icons-material";
import { Link, NavLink } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../app/store/store";
import { setDarkMode } from "./uiSlice";
import { useBasket } from "../../hooks/useBasket";
import UserMenu from "./UserMenu";
import { useUserInfoQuery } from "../../features/authentication/services/account.api";
import logo from "../../assets/logo.png";

const leftLinks = [
    { title: "home", path: "/" },
    { title: "products", path: "/products" },
    { title: "inventory", path: "/inventory" },
    { title: "contact", path: "/contact" },
];

const rightLinks = [
    { title: "login", path: "/login" },
    { title: "register", path: "/register" },
];

const navStyles = {
    color: "inherit",
    typography: "h6",
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
        <AppBar position="fixed" color="primary">
            <Container maxWidth="xl">
                <Toolbar sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }} disableGutters>
                    <Box display="flex" alignItems="center">
                        <Typography component={NavLink} to="/">
                            <Box
                                component="img"
                                sx={{
                                    height: 50,
                                    width: 160,
                                }}
                                alt="ego store logo."
                                src={logo}
                            />
                        </Typography>
                        <List sx={{ display: "flex" }}>
                            {leftLinks.map(({ title, path }) => (
                                <ListItem component={NavLink} to={path} key={path} sx={navStyles}>
                                    {title.toUpperCase()}
                                </ListItem>
                            ))}
                        </List>
                    </Box>

                    <Box display="flex" alignItems="center">
                        <IconButton component={Link} to="/basket" size="large" sx={{ color: "inherit" }}>
                            <Badge badgeContent={itemCount} color="secondary">
                                <ShoppingCart />
                            </Badge>
                        </IconButton>
                        <IconButton onClick={() => dispatch(setDarkMode())}>{darkMode ? <LightMode sx={{ color: "yellow" }} /> : <DarkMode sx={{ color: "darkgray" }} />}</IconButton>
                        {user ? (
                            <UserMenu user={user} />
                        ) : (
                            <List sx={{ display: "flex" }}>
                                {rightLinks.map(({ title, path }) => (
                                    <ListItem component={NavLink} to={path} key={path} sx={navStyles}>
                                        {title.toUpperCase()}
                                    </ListItem>
                                ))}
                            </List>
                        )}
                    </Box>
                </Toolbar>
            </Container>

            {isLoading && (
                <Box sx={{ width: "100%" }}>
                    <LinearProgress color="secondary" />
                </Box>
            )}
        </AppBar>
    );
}
