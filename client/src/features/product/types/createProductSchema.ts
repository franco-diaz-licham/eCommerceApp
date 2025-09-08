import { z } from "zod";

/** Schema validation for new product. */
export const createProductSchema = z.object({
    name: z.string().min(1, { message: "Name of product is required" }),
    description: z.string().min(10, { message: "Description must be at least 10 characters" }),
    price: z.number().min(1, { message: "Price must be at least $1.00" }),
    type: z.number({ message: "Type is required" }).int().positive(),
    brand: z.number({ message: "Brand is required" }).int().positive(),
    quantityInStock: z.number().int().min(1, { message: "Quantity must be at least 1" }),
    pictureUrl: z.string().url().optional(),
    photo: z.instanceof(File).optional(),
});

export type CreateProductSchema = z.infer<typeof createProductSchema>;
