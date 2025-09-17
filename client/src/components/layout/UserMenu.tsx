import { Button, Menu, Fade, MenuItem, ListItemIcon, ListItemText, Divider } from "@mui/material";
import { useState } from "react";
import { History,  Inventory,  Logout, Person } from "@mui/icons-material";
import { Link } from "react-router-dom";
import type { UserResponse } from "../../features/authentication/types/user.type";
import { useSignOutMutation } from "../../features/authentication/services/account.api";

type UserMenuProps = {
    user: UserResponse
}

export default function UserMenu({ user }: UserMenuProps) {
    const [signOut] = useSignOutMutation();
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);
    const handleClick = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };
    const handleClose = () => {
        setAnchorEl(null);
    };

    return (
        <div>
            <Button
                onClick={handleClick}
                color='inherit'
                variant="text"
                sx={{fontSize: '1.1rem', textTransform: "none"}}
            >
                {user.email}
            </Button>
            <Menu
                id="fade-menu"
                MenuListProps={{
                    'aria-labelledby': 'fade-button',
                }}
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose} disableScrollLock
                TransitionComponent={Fade}
            >
                <MenuItem>
                    <ListItemIcon>
                        <Person />
                    </ListItemIcon>
                    <ListItemText>My profile</ListItemText>
                </MenuItem>
                <MenuItem component={Link} to=''>
                    <ListItemIcon>
                        <History />
                    </ListItemIcon>
                    <ListItemText>My orders</ListItemText>
                </MenuItem>
                {user.roles?.includes('Admin') &&
                <MenuItem component={Link} to='/inventory'>
                    <ListItemIcon>
                        <Inventory />
                    </ListItemIcon>
                    <ListItemText>Inventory</ListItemText>
                </MenuItem>}
                <Divider />
                <MenuItem onClick={signOut}>
                    <ListItemIcon>
                        <Logout />
                    </ListItemIcon>
                    <ListItemText>Logout</ListItemText>
                </MenuItem>
            </Menu>
        </div>
    );
}