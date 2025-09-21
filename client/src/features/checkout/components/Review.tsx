import { Box, Divider, Table, TableBody, TableCell, TableContainer, TableRow, Typography } from "@mui/material";
import { useBasket } from "../../../hooks/useBasket";
import { currencyFormat, formatAddressString, formatPaymentString } from "../../../lib/utils";
import { type ConfirmationToken } from "@stripe/stripe-js";

interface ReviewProps {
    confirmationToken: ConfirmationToken | null;
}

export default function Review({ confirmationToken }: ReviewProps) {
    const { basket } = useBasket();

    /** Format address for diplaying */
    const addressString = () => {
        if (!confirmationToken?.shipping) return "";
        const { name, address } = confirmationToken.shipping;
        if (!address) return "Problem displaying address";
        return formatAddressString(address.line1, address.city, address.state, address.postal_code, address.country, name);
    };

    /** Get payment string hiding sensitive details. */
    const paymentString = () => {
        if (!confirmationToken?.payment_method_preview.card) return "";
        const { card } = confirmationToken.payment_method_preview;
        return formatPaymentString(card.brand, card.last4, card.exp_month, card.exp_year)
    };

    return (
        <div>
            <Box mt={4} width="100%">
                <Typography variant="h6" fontWeight="bold">
                    Billing and delivery information
                </Typography>
                <dl>
                    <Typography component="dt" fontWeight="medium">
                        Shipping address
                    </Typography>
                    <Typography component="dd" mt={1} color="textSecondary">
                        {addressString()}
                    </Typography>

                    <Typography component="dt" fontWeight="medium">
                        Payment details
                    </Typography>
                    <Typography component="dd" mt={1} color="textSecondary">
                        {paymentString()}
                    </Typography>
                </dl>
            </Box>

            <Box mt={6} mx="auto">
                <Divider />
                <TableContainer>
                    <Table>
                        <TableBody>
                            {basket?.basketItems.map((item) => (
                                <TableRow key={item.productId} sx={{ borderBottom: "1px solid rgba(224, 224, 224, 1)" }}>
                                    <TableCell sx={{ py: 4 }}>
                                        <Box display="flex" gap={3} alignItems="center">
                                            <img src={item.publicUrl} alt={item.name} style={{ width: 40, height: 40 }} />
                                            <Typography>{item.name}</Typography>
                                        </Box>
                                    </TableCell>
                                    <TableCell align="center" sx={{ p: 4 }}>
                                        x {item.quantity}
                                    </TableCell>
                                    <TableCell align="right" sx={{ p: 4 }}>
                                        {currencyFormat(item.unitPrice)}
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Box>
        </div>
    );
}
