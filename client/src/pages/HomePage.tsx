import * as React from "react";
import { AppBar, Toolbar, Container, Box, Typography, Button, IconButton, Badge, Grid, Card, CardMedia, CardContent, CardActions, Chip, Stack, Divider, Paper, TextField, Link as MuiLink } from "@mui/material";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";
import RestartAltIcon from "@mui/icons-material/RestartAlt";
import SupportAgentIcon from "@mui/icons-material/SupportAgent";

// ---------------------- Mock Data ----------------------
type Product = {
    id: number;
    title: string;
    price: number;
    image: string;
    tag?: string;
};

const FEATURED_PRODUCTS: Product[] = [
    { id: 1, title: "Classic White T-Shirt", price: 29.99, image: "https://images.unsplash.com/photo-1520975916090-3105956dac38?auto=format&fit=crop&w=1200&q=60" },
    { id: 2, title: "Black Slim Fit Jeans", price: 59.5, image: "https://images.unsplash.com/photo-1516826957135-700dedea698c?auto=format&fit=crop&w=1200&q=60", tag: "Hot" },
    { id: 3, title: "Blue Denim Jacket", price: 89.99, image: "https://images.unsplash.com/photo-1516822003754-cca485356ecb?auto=format&fit=crop&w=1200&q=60" },
    { id: 4, title: "Casual Grey Hoodie", price: 49.99, image: "https://images.unsplash.com/photo-1512436991641-6745cdb1723f?auto=format&fit=crop&w=1200&q=60", tag: "New" },
    { id: 5, title: "Leather Biker Jacket", price: 199.99, image: "https://images.unsplash.com/photo-1543087903-1ac2ec7aa8c5?auto=format&fit=crop&w=1200&q=60" },
    { id: 6, title: "Navy Chinos", price: 54.75, image: "https://images.unsplash.com/photo-1539533113208-f6df8cc8b543?auto=format&fit=crop&w=1200&q=60" },
];

const CATEGORIES = [
    { label: "Hoodies", image: "https://images.unsplash.com/photo-1516822003754-cca485356ecb?auto=format&fit=crop&w=1200&q=60" },
    { label: "Jackets", image: "https://images.unsplash.com/photo-1543087903-1ac2ec7aa8c5?auto=format&fit=crop&w=1200&q=60" },
    { label: "Dresses", image: "https://images.unsplash.com/photo-1503342217505-b0a15cf70489?auto=format&fit=crop&w=1200&q=60" },
    { label: "Shirts", image: "https://images.unsplash.com/photo-1520975916090-3105956dac38?auto=format&fit=crop&w=1200&q=60" },
];

const BRANDS = ["Nike", "Uniqlo", "H&M", "Zara", "Levi’s", "Calvin Klein", "Ralph Lauren", "Patagonia"];

// ---------------------- Small Components ----------------------
function ProductCard({ product }: { product: Product }) {
    return (
        <Card
            elevation={0}
            sx={{
                borderRadius: 3,
                border: (t) => `1px solid ${t.palette.divider}`,
                "&:hover": { boxShadow: 4, transform: "translateY(-2px)" },
                transition: "all .18s ease",
            }}
        >
            <Box sx={{ position: "relative" }}>
                {product.tag && <Chip color="secondary" label={product.tag} size="small" sx={{ position: "absolute", top: 12, left: 12, zIndex: 1, fontWeight: 600 }} />}
                <CardMedia sx={{ height: 220 }} image={product.image} title={product.title} />
            </Box>
            <CardContent sx={{ pb: 1 }}>
                <Typography variant="subtitle1" fontWeight={600} noWrap title={product.title}>
                    {product.title}
                </Typography>
                <Typography variant="body1" color="secondary" fontWeight={700}>
                    ${product.price.toFixed(2)}
                </Typography>
            </CardContent>
            <CardActions sx={{ px: 2, pb: 2 }}>
                <Button variant="contained" size="small" fullWidth>
                    Add to cart
                </Button>
                <Button size="small">View</Button>
            </CardActions>
        </Card>
    );
}

function ValueProp({ icon, title, subtitle }: { icon: React.ReactNode; title: string; subtitle: string }) {
    return (
        <Stack direction="row" spacing={2} alignItems="center">
            <Box sx={{ p: 1, borderRadius: 2, bgcolor: "action.hover" }}>{icon}</Box>
            <Box>
                <Typography fontWeight={700}>{title}</Typography>
                <Typography variant="body2" color="text.secondary">
                    {subtitle}
                </Typography>
            </Box>
        </Stack>
    );
}

// ---------------------- Main Page ----------------------
export default function HomePage() {
    // Pretend cart size from state
    const cartCount = 2;

    return (
        <Box>
            {/* Top App Bar */}
            <AppBar position="sticky" color="primary" elevation={0}>
                <Toolbar sx={{ gap: 2 }}>
                    <Typography variant="h6" sx={{ fontWeight: 800, letterSpacing: 1 }}>
                        e<span style={{ fontStyle: "italic" }}>go</span>
                    </Typography>

                    <Stack direction="row" spacing={2} sx={{ ml: 3, display: { xs: "none", md: "flex" } }}>
                        <MuiLink href="#" color="inherit" underline="none">
                            Products
                        </MuiLink>
                        <MuiLink href="#" color="inherit" underline="none">
                            Inventory
                        </MuiLink>
                        <MuiLink href="#" color="inherit" underline="none">
                            About
                        </MuiLink>
                        <MuiLink href="#" color="inherit" underline="none">
                            Contact
                        </MuiLink>
                    </Stack>

                    <Box sx={{ flexGrow: 1 }} />
                    <IconButton color="inherit" aria-label="toggle dark mode">
                        <DarkModeIcon />
                    </IconButton>
                    <IconButton color="inherit" aria-label="cart">
                        <Badge badgeContent={cartCount} color="secondary">
                            <ShoppingCartIcon />
                        </Badge>
                    </IconButton>
                    <Button color="inherit" size="small" sx={{ textTransform: "none" }}>
                        test@test.com
                    </Button>
                </Toolbar>
            </AppBar>

            {/* Hero */}
            <Box
                sx={(t) => ({
                    background: `linear-gradient(135deg, ${t.palette.primary.light} 0%, ${t.palette.info.light} 45%, ${t.palette.background.default} 100%)`,
                    py: { xs: 8, md: 12 },
                    borderBottom: `1px solid ${t.palette.divider}`,
                })}
            >
                <Container maxWidth="lg">
                    <Grid container spacing={6} alignItems="center">
                        <Grid >
                            <Typography variant="h2" fontWeight={800} gutterBottom>
                                Bring your style to life
                            </Typography>
                            <Typography variant="h6" color="text.secondary" sx={{ mb: 3 }}>
                                Discover curated essentials for every season. Quality pieces, sharp prices, fast delivery.
                            </Typography>
                            <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                                <Button variant="contained" size="large" endIcon={<ArrowForwardIcon />}>
                                    Shop new in
                                </Button>
                                <Button variant="outlined" size="large">
                                    Explore all products
                                </Button>
                            </Stack>

                            <Stack direction={{ xs: "column", sm: "row" }} spacing={3} sx={{ mt: 5 }}>
                                <ValueProp icon={<LocalShippingIcon />} title="Free Shipping" subtitle="On orders over $75" />
                                <ValueProp icon={<RestartAltIcon />} title="Free Returns" subtitle="30-day policy" />
                                <ValueProp icon={<SupportAgentIcon />} title="Support" subtitle="7 days a week" />
                            </Stack>
                        </Grid>

                        <Grid>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 2,
                                    borderRadius: 3,
                                    border: (t) => `1px solid ${t.palette.divider}`,
                                    bgcolor: "background.paper",
                                }}
                            >
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Trending brands
                                </Typography>
                                <Stack direction="row" flexWrap="wrap" gap={1.2}>
                                    {BRANDS.map((b) => (
                                        <Chip key={b} label={b} clickable />
                                    ))}
                                </Stack>
                                <Divider sx={{ my: 2 }} />
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Quick search
                                </Typography>
                                <Stack direction={{ xs: "column", sm: "row" }} spacing={1.5}>
                                    <TextField fullWidth placeholder="Search products" size="small" />
                                    <Button variant="contained" sx={{ whiteSpace: "nowrap" }}>
                                        Search
                                    </Button>
                                </Stack>
                            </Paper>
                        </Grid>
                    </Grid>
                </Container>
            </Box>

            {/* Featured Categories */}
            <Container maxWidth="lg" sx={{ py: { xs: 6, md: 8 } }}>
                <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 2 }}>
                    <Typography variant="h5" fontWeight={800}>
                        Shop by Category
                    </Typography>
                    <Button size="small" endIcon={<ArrowForwardIcon />}>
                        View all
                    </Button>
                </Stack>

                <Grid container spacing={2.5}>
                    {CATEGORIES.map((cat) => (
                        <Grid key={cat.label} >
                            <Paper
                                variant="outlined"
                                sx={{
                                    borderRadius: 3,
                                    overflow: "hidden",
                                    "&:hover": { boxShadow: 3 },
                                    transition: "box-shadow .18s ease",
                                }}
                            >
                                <Box sx={{ height: 140, backgroundImage: `url(${cat.image})`, backgroundSize: "cover", backgroundPosition: "center" }} />
                                <Box sx={{ p: 2 }}>
                                    <Typography fontWeight={700}>{cat.label}</Typography>
                                    <Button size="small" endIcon={<ArrowForwardIcon />} sx={{ mt: 0.5 }}>
                                        Shop now
                                    </Button>
                                </Box>
                            </Paper>
                        </Grid>
                    ))}
                </Grid>
            </Container>

            {/* Featured Products */}
            <Container maxWidth="lg" sx={{ pb: { xs: 6, md: 8 } }}>
                <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 2 }}>
                    <Typography variant="h5" fontWeight={800}>
                        Featured Products
                    </Typography>
                    <Button size="small" endIcon={<ArrowForwardIcon />}>
                        See more
                    </Button>
                </Stack>

                <Grid container spacing={2.5}>
                    {FEATURED_PRODUCTS.map((p) => (
                        <Grid key={p.id}>
                            <ProductCard product={p} />
                        </Grid>
                    ))}
                </Grid>
            </Container>

            {/* Promo Banner */}
            <Container maxWidth="lg" sx={{ pb: { xs: 6, md: 8 } }}>
                <Paper
                    sx={(t) => ({
                        p: { xs: 3, md: 5 },
                        borderRadius: 3,
                        background: `linear-gradient(135deg, ${t.palette.success.light} 0%, ${t.palette.info.light} 100%)`,
                        color: "black",
                    })}
                >
                    <Grid container spacing={3} alignItems="center">
                        <Grid>
                            <Typography variant="h5" fontWeight={900}>
                                Spring Sale — up to 40% off
                            </Typography>
                            <Typography color="text.primary" sx={{ opacity: 0.9 }}>
                                Limited time offers on essentials. Free shipping over $75.
                            </Typography>
                        </Grid>
                        <Grid >
                            <Stack direction={{ xs: "column", sm: "row" }} spacing={1.5} justifyContent="flex-end">
                                <Button variant="contained" color="inherit">
                                    Shop Men
                                </Button>
                                <Button variant="outlined" color="inherit">
                                    Shop Women
                                </Button>
                            </Stack>
                        </Grid>
                    </Grid>
                </Paper>
            </Container>

            {/* Newsletter */}
            <Container maxWidth="lg" sx={{ pb: { xs: 6, md: 10 } }}>
                <Paper variant="outlined" sx={{ p: { xs: 3, md: 5 }, borderRadius: 3 }}>
                    <Grid container spacing={3} alignItems="center">
                        <Grid >
                            <Typography variant="h6" fontWeight={800}>
                                Get 10% off your first order
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                Join our newsletter for exclusive drops and early access.
                            </Typography>
                        </Grid>
                        <Grid >
                            <Stack direction={{ xs: "column", sm: "row" }} spacing={1.5}>
                                <TextField fullWidth placeholder="Enter your email" />
                                <Button variant="contained">Subscribe</Button>
                            </Stack>
                        </Grid>
                    </Grid>
                </Paper>
            </Container>

            {/* Footer */}
            <Box sx={{ bgcolor: "background.paper", borderTop: (t) => `1px solid ${t.palette.divider}` }}>
                <Container maxWidth="lg" sx={{ py: 4 }}>
                    <Grid container spacing={3}>
                        <Grid >
                            <Typography variant="h6" fontWeight={800}>
                                e<span style={{ fontStyle: "italic" }}>go</span>
                            </Typography>
                            <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                                Designed with Material UI. © {new Date().getFullYear()} eGo Store. All rights reserved.
                            </Typography>
                        </Grid>
                        <Grid>
                            <Stack direction="row" spacing={3} justifyContent={{ xs: "flex-start", md: "flex-end" }}>
                                <MuiLink href="#" color="text.secondary" underline="hover">
                                    Privacy
                                </MuiLink>
                                <MuiLink href="#" color="text.secondary" underline="hover">
                                    Terms
                                </MuiLink>
                                <MuiLink href="#" color="text.secondary" underline="hover">
                                    Support
                                </MuiLink>
                            </Stack>
                        </Grid>
                    </Grid>
                </Container>
            </Box>
        </Box>
    );
}
