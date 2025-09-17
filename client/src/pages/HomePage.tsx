import { Container, Box, Typography, Button, Grid, Card, CardMedia, CardContent, CardActions, Chip, Stack, Paper } from "@mui/material";
import Hero from "../features/home/components/Hero";
import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import NewsLetter from "../features/home/components/NewLetters";

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

// ---------------------- Main Page ----------------------
export default function HomePage() {
    return (
        <>
            {/* Hero */}
            <Hero />

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
                        <Grid key={cat.label}>
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
                        <Grid>
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
            <NewsLetter />
        </>
    );
}
