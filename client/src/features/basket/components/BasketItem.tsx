import { Box, Grid, IconButton, Paper, Typography } from "@mui/material"
import { Add, Close, Remove } from "@mui/icons-material"
import { currencyFormat } from "../../../lib/utils"
import { useAddBasketItemMutation, useRemoveBasketItemMutation } from "../services/basketApi"
import type { BasketItemResponse } from "../types/basket.type"

type Props = {
    item: BasketItemResponse
}

export default function BasketItem({ item }: Props) {
    const [removeBasketItem] = useRemoveBasketItemMutation();
    const [addBasketItem] = useAddBasketItemMutation();

    return (
        <Paper sx={{
            height: 140,
            borderRadius: 3,
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            mb: 2
        }}>
            <Box display='flex' alignItems='center'>
                <Box
                    component='img'
                    src={item.pictureUrl}
                    alt={item.name}
                    sx={{
                        width: 100,
                        height: 100,
                        objectFit: 'cover',
                        borderRadius: '4px',
                        mr: 8,
                        ml: 4
                    }}
                />

                <Box display='flex' flexDirection='column' gap={1}>
                    <Typography variant="h6">{item.name}</Typography>

                    <Box display='flex' alignItems='center' gap={3}>
                        <Typography sx={{fontSize: '1.1rem'}}>
                            {currencyFormat(item.price)} x {item.quantity}
                        </Typography>
                        <Typography sx={{fontSize: '1.1rem'}} color='primary'>
                            {currencyFormat(item.price * item.quantity)}
                        </Typography>
                    </Box>

                    <Grid container spacing={1} alignItems='center'>
                        <IconButton 
                            onClick={() => removeBasketItem({productId: item.productId, quantity: 1, basketId: item.basketId})}
                            color="error" 
                            size="small" 
                            sx={{border: 1, borderRadius: 1, minWidth: 0}}
                        >
                            <Remove />
                        </IconButton>
                        <Typography variant="h6">{item.quantity}</Typography>
                        <IconButton 
                            onClick={() => addBasketItem({productId: item.productId, quantity: 1, basketId: item.basketId})}
                            color="success" 
                            size="small" 
                            sx={{border: 1, borderRadius: 1, minWidth: 0}}
                        >
                            <Add />
                        </IconButton>
                    </Grid>
                </Box>
            </Box>
            <IconButton
                onClick={() => removeBasketItem({productId: item.productId, quantity: item.quantity, basketId: item.basketId})}
                color='error'
                size="small" 
                sx={{
                    border: 1, 
                    borderRadius: 1, 
                    minWidth: 0, 
                    alignSelf: 'start',
                    mr: 1,
                    mt: 1
                }}
            >
                <Close />
            </IconButton>
        </Paper>
    )
}