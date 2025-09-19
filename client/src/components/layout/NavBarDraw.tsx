import { Box, Divider, Drawer, List, ListItem, ListItemButton, ListItemText, Typography } from "@mui/material";
import DarkLogo from "../../assets/dark-logo.png";
import { NavLink, useNavigate } from "react-router-dom";
import type { NavBarLink } from "./NavBar";
import type { UserResponse } from "../../features/authentication/types/user.type";

interface NavBarDrawProps {
    links: NavBarLink[];
    authLinks: NavBarLink[];
    drawerOpen: boolean;
    user?: UserResponse;
    window?: () => Window;
    closeDraw: () => void;
}

export default function NavBarDraw(props: NavBarDrawProps) {
    const navigate = useNavigate();
    const { window } = props;
    const container = window !== undefined ? () => window().document.body : undefined;

    return (
        <Drawer
            container={container}
            variant="temporary"
            open={props.drawerOpen}
            onClose={props.closeDraw}
            ModalProps={{
                keepMounted: true,
            }}
            sx={{
                display: { xs: "block", sm: "none" },
                "& .MuiDrawer-paper": { boxSizing: "border-box", width: 240 },
            }}
        >
            <Box onClick={props.closeDraw} sx={{ textAlign: "center" }}>
                <Typography component={NavLink} to="/">
                    <Box
                        component="img"
                        sx={{
                            height: 50,
                            width: 160,
                        }}
                        alt="ego store logo."
                        src={DarkLogo}
                    />
                </Typography>
                <Divider />
                <List>
                    {props.links.map((item, index) => (
                        <ListItem key={index} disablePadding>
                            <ListItemButton sx={{ textAlign: "center" }} onClick={() => navigate(item.path)}>
                                <ListItemText primary={item.title} />
                            </ListItemButton>
                        </ListItem>
                    ))}
                    {!props.user &&
                        props.links.map((item, index) => (
                            <ListItem key={index} disablePadding>
                                <ListItemButton sx={{ textAlign: "center" }} onClick={() => navigate(item.path)}>
                                    <ListItemText primary={item.title} />
                                </ListItemButton>
                            </ListItem>
                        ))}
                </List>
            </Box>
        </Drawer>
    );
}
