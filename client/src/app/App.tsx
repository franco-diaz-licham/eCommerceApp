import { Outlet, ScrollRestoration } from "react-router-dom";
import "./styles/App.css";
import { useAppSelector } from "./store/store";
import { Box, Container, createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import NavBar from "../components/layout/NavBar";
import Footer from "../components/layout/Footer";

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
                    minHeight: "calc(100vh - 190px)",
                    background: darkMode ? "#070d27ff" : "#f6fcffff",
                    pb: 4,
                }}
            >
                <Container maxWidth="xl">
                    <Outlet />
                </Container>
            </Box>
            <Footer />
        </ThemeProvider>
    );
}

export default App;
