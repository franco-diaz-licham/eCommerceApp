import { FormGroup, FormControlLabel, Checkbox } from "@mui/material";
import { useEffect, useState } from "react";

/** Option model for selector */
type Option = { value: number; label: string };

type CheckboxButtonsProps = {
    items: Option[];
    checked: Option[];
    onChange: (items: Option[]) => void;
};

export default function CheckboxButtons(props: CheckboxButtonsProps) {
    const [checkedItems, setCheckedItems] = useState(props.checked);
    const checkItemsIds = checkedItems?.map((x) => x.value);
    useEffect(() => {
        setCheckedItems(props.checked);
    }, [props.checked]);

    const handleToggle = (value: Option) => {
        const updatedChecked = checkItemsIds.includes(value.value) ? checkedItems.filter((item) => item.value !== value.value) : [...checkedItems, value];
        setCheckedItems(updatedChecked);
        props.onChange(updatedChecked);
    };

    return (
        <FormGroup>
            {props.items.map((item) => (
                <FormControlLabel key={item.value} control={<Checkbox checked={checkItemsIds.includes(item.value)} onClick={() => handleToggle(item)} color="secondary" sx={{ py: 0.7, fontSize: 40 }} />} label={item.label} />
            ))}
        </FormGroup>
    );
}
