import { UploadFile } from "@mui/icons-material";
import { FormControl, FormHelperText, Typography } from "@mui/material";
import { useCallback } from "react";
import { useDropzone, type DropzoneProps } from "react-dropzone";
import { type FieldValues, useController, type UseControllerProps } from "react-hook-form";

/** Functional props. Add base RHF and MUI props. */
type DropZoneProps<T extends FieldValues> = {
    name: keyof T;
} & UseControllerProps<T> &
    Partial<DropzoneProps>;

export default function Dropzone<T extends FieldValues>(props: DropZoneProps<T>) {
    const { fieldState, field } = useController({ ...props });

    const onDrop = useCallback(
        (acceptedFiles: File[]) => {
            if (acceptedFiles.length > 0) {
                const fileWithPreview = Object.assign(acceptedFiles[0], {
                    preview: URL.createObjectURL(acceptedFiles[0]),
                });

                field.onChange(fileWithPreview);
            }
        },
        [field]
    );

    const { getRootProps, getInputProps, isDragActive } = useDropzone({ onDrop });

    const dzStyles = {
        display: "flex",
        border: "dashed 2px #c7c7c7ff",
        borderRadius: "5px",
        paddingTop: "30px",
        alignItems: "center",
        height: 100,
    };

    const dzActive = {
        borderColor: "green",
    };

    return (
        <div {...getRootProps()} style={{marginTop: 20}}>
            <FormControl style={isDragActive ? { ...dzStyles, ...dzActive } : dzStyles} error={!!fieldState.error}>
                <input {...getInputProps()} />
                <UploadFile sx={{ fontSize: "30px" }} />
                <Typography >Drop photo here</Typography>
                <FormHelperText>{fieldState.error?.message}</FormHelperText>
            </FormControl>
        </div>
    );
}
