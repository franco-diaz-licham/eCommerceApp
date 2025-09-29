import z from "zod";

/** Zod login schema. */
export const profileSchema = z.object({
    line1: z.string().nonempty().min(6),
    line2: z.string().nullable(),
    city: z.string(),
    state: z.string(),
    postalCode: z.string(),
    country: z.string(),
});

export type ProfileSchema = z.infer<typeof profileSchema>;
