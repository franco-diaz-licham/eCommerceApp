import { Box, Pagination, Typography } from "@mui/material";
import type { Pagination as PaginationType } from "../../types/pagination.type";

type AppPaginationProps = {
    metadata: PaginationType;
    onPageChange: (page: number) => void;
};

export default function AppPagination({ metadata, onPageChange }: AppPaginationProps) {
    const { pageNumber, totalPages, pageSize, totalCount } = metadata;
    const startItem = (pageNumber - 1) * pageSize + 1;
    const endItem = Math.min(pageNumber * pageSize, totalCount);

    return (
        <Box display="flex" justifyContent="center" alignItems="center" marginTop={3} flexWrap={"wrap"}>
            <Typography>
                Displaying {startItem}-{endItem} of {totalCount} items
            </Typography>
            <Pagination color="secondary" size="large" count={totalPages} page={pageNumber} onChange={(_, page) => onPageChange(page)} />
        </Box>
    );
}
