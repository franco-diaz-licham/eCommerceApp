import { Outlet, ScrollRestoration } from "react-router-dom";
import "./styles/App.css";
import { useAppSelector } from "./store/store";
import { Box, Container, createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import NavBar from "../components/layout/NavBar";

function App() {
    const { darkMode } = useAppSelector((state) => state.ui);
    const palleteType = darkMode ? "dark" : "light";
    const theme = createTheme({
        palette: {
            mode: palleteType,
            background: {
                default: palleteType === "dark" ? "#070d27ff" : "#e2f6ffff",
            },
        },
    });

    return (
        <ThemeProvider theme={theme}>
            <ScrollRestoration />
            <CssBaseline />
            <NavBar />
            <Box
                sx={{
                    minHeight: "100vh",
                    background: darkMode ? "#070d27ff" : "#f6fcffff",
                    py: 4,
                }}
            >
                <Container maxWidth="xl" sx={{ mt: 8 }}>
                    <Outlet />
                </Container>
            </Box>
        </ThemeProvider>
    );
}

export default App;
